namespace Bars.Gkh.Ris.Integration.Signature
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Xml;

    public class GisIntegrationClientMessageInspector : IClientMessageInspector, IEndpointBehavior
    {
        public GisIntegrationClientMessageInspector(X509Certificate2 certificate)
        {
            this.Certificate = certificate;
        }

        private X509Certificate2 Certificate { get; set; }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(request.ToString());

            var signedXml = new RisSignedXml(doc);
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(Certificate));
            var reference = new Reference("#block-to-sign");
            reference.DigestMethod = "http://www.w3.org/2001/04/xmldsig-more#gostr3411";
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            var excC14Ntransform = new XmlDsigExcC14NTransform();
            reference.AddTransform(excC14Ntransform);
            signedXml.AddReference(reference);
            signedXml.Signature.Id = "xmldsig-" + Guid.NewGuid();
            signedXml.SignedInfo.CanonicalizationMethod = excC14Ntransform.Algorithm;
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#gostr34102001-gostr3411";
            signedXml.SigningKey = Certificate.PrivateKey;

            XadesSign.AddXAdESNodes(signedXml, doc, Certificate, "http://www.w3.org/2001/04/xmldsig-more#gostr3411");

            signedXml.ComputeSignature();

            if (!signedXml.CheckSignature())
            {
                throw new Exception("Не удалось подписать сообщение");
            }

            var signatureBlockXml = signedXml.GetXml();
            var blockToSign = signedXml.GetIdElement(doc, "block-to-sign");

            if (blockToSign == null)
            {
                throw new Exception("Не удалось подписать сообщение");
            }

            blockToSign.InsertBefore(signatureBlockXml, blockToSign.FirstChild);

            var quotas = new XmlDictionaryReaderQuotas();
            var encoderWithoutBOM = new UTF8Encoding(false);
            var ms = new System.IO.MemoryStream(encoderWithoutBOM.GetBytes(doc.InnerXml));
            var xdr = XmlDictionaryReader.CreateTextReader(ms, encoderWithoutBOM, quotas, null);

            var newMessage = Message.CreateMessage(xdr, int.MaxValue, request.Version);
            request = newMessage;

            return null;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class RisSignedXml : SignedXml
    {
        public Dictionary<string, XmlElement> ElementsById { get; set; }

        public RisSignedXml(XmlDocument doc)
            : base(doc)
        {
            ElementsById = new Dictionary<string, XmlElement>();
        }

        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            XmlElement result;
            if (ElementsById.TryGetValue(idValue, out result))
            {
                return result;
            }
            return base.GetIdElement(document, idValue);
        }
    }
}