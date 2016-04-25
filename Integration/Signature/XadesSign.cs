namespace Bars.Gkh.Ris.Integration.Signature
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Xml;

    public class XadesSign
    {
        public const string XadesNamespaceUrl = "http://uri.etsi.org/01903/v1.3.2#";
        public const string XmlDsigSignatureProperties = "http://uri.etsi.org/01903#SignedProperties";

        public static void AddXAdESNodes(RisSignedXml signedXml, XmlDocument document, X509Certificate2 cert, string digestMethod)
        {
            var qualifyingPropertiesNode = AddQualifyingPropertiesNode(signedXml, document);
            var signedPropertiesNode = AddSignedPropertiesNode(signedXml, document, qualifyingPropertiesNode);
            CreateReferenceToSignedProperties(signedXml, signedPropertiesNode, digestMethod);
            var signedSignatureProperties = AddSignedSignaturePropertiesNode(document, signedPropertiesNode);
            AddSigningTimeNode(document, signedSignatureProperties);
            AddSigningCertificate(document, signedSignatureProperties, cert, digestMethod);
        }

        private static XmlElement AddQualifyingPropertiesNode(RisSignedXml signedXml, XmlDocument document)
        {
            var result = document.CreateElement("QualifyingProperties", XadesNamespaceUrl);
            result.SetAttribute("Target", string.Format("#{0}", signedXml.Signature.Id));
            var dataObject = new DataObject(null, null, null, result);
            signedXml.AddObject(dataObject);
            return result;
        }

        private static XmlElement AddSignedPropertiesNode(RisSignedXml signedXml, XmlDocument document, XmlElement qualifyingPropertiesNode)
        {
            var signedPropertiesNode = CreateNodeIn(document, "SignedProperties", XadesNamespaceUrl, qualifyingPropertiesNode);
            var id = "_" + Guid.NewGuid().ToString();
            signedPropertiesNode.SetAttribute("Id", id);

            signedXml.ElementsById.Add(id, signedPropertiesNode);

            return signedPropertiesNode;
        }

        private static void CreateReferenceToSignedProperties(RisSignedXml signedXml, XmlElement signedPropertiesNode, string digestMethod)
        {
            var reference = new Reference("#" + signedPropertiesNode.GetAttribute("Id"))
            {
                Type = XmlDsigSignatureProperties,
                DigestMethod = digestMethod,
            };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigExcC14NTransform());
            signedXml.AddReference(reference);
        }

        private static XmlElement AddSignedSignaturePropertiesNode(XmlDocument document, XmlElement propertiesNode)
        {
            return CreateNodeIn(document, "SignedSignatureProperties", XadesNamespaceUrl, propertiesNode);
        }

        private static void AddSigningTimeNode(XmlDocument document, XmlElement signedSignaturePropertiesNode)
        {
            CreateNodeWithTextIn(document, "SigningTime", NowInCanonicalRepresentation(),
                XadesNamespaceUrl, signedSignaturePropertiesNode);
        }

        private static void AddSigningCertificate(XmlDocument document, XmlElement signedSignatureProperties, X509Certificate2 cert, string digestMethod)
        {
            var signingCertificateNode = CreateNodeIn(document, "SigningCertificate", XadesNamespaceUrl, signedSignatureProperties);
            var certNode = CreateNodeIn(document, "Cert", XadesNamespaceUrl, signingCertificateNode);
            AddCertDigestNode(document, certNode, cert, digestMethod/*, parameters*/);
            AddIssuerSerialNode(document, certNode, cert/*, parameters*/);
        }

        private static XmlElement CreateNodeIn(XmlDocument document, string nodeName, string nameSpace, XmlElement rootNode)
        {
            var result = document.CreateElement(nodeName, nameSpace);
            rootNode.AppendChild(result);
            return result;
        }

        private static XmlElement CreateNodeWithTextIn(XmlDocument document, string nodeName, string text, string nameSpace, XmlElement rootNode)
        {
            var newNode = CreateNodeIn(document, nodeName, nameSpace, rootNode);
            newNode.InnerText = text;
            return newNode;
        }

        private static string DateTimeToCanonicalRepresentation(DateTime ahora)
        {
            return ahora.Year.ToString("0000") + "-" + ahora.Month.ToString("00") +
                   "-" + ahora.Day.ToString("00") +
                   "T" + ahora.Hour.ToString("00") + ":" +
                   ahora.Minute.ToString("00") + ":" + ahora.Second.ToString("00") +
                   "Z";
        }

        private static string NowInCanonicalRepresentation()
        {
            var now = DateTime.Now.ToUniversalTime();
            return DateTimeToCanonicalRepresentation(now);
        }
        private static void AddCertDigestNode(XmlDocument document, XmlElement certNode, X509Certificate2 cert, string digestMethod)
        {
            var certDigestNode = CreateNodeIn(document, "CertDigest", XadesNamespaceUrl, certNode);
            var digestMethodNode = CreateNodeIn(document, "DigestMethod", SignedXml.XmlDsigNamespaceUrl, certDigestNode);
            digestMethodNode.SetAttribute("Algorithm", digestMethod);
            var certificateData = cert.Export(X509ContentType.Cert);
            byte[] hash;
            using (var hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(digestMethod))
            {
                hash = hashAlgorithm.ComputeHash(certificateData);
            }
            var digestValue = Convert.ToBase64String(hash);
            CreateNodeWithTextIn(document, "DigestValue", digestValue, SignedXml.XmlDsigNamespaceUrl, certDigestNode);
        }

        private static void AddIssuerSerialNode(XmlDocument document, XmlElement certNode, X509Certificate2 cert)
        {
            var issuerSerialNode = CreateNodeIn(document, "IssuerSerial", XadesNamespaceUrl, certNode);
            CreateNodeWithTextIn(document, "X509IssuerName", "cn=CIT RT CA,ou=Удостоверяющий центр,o=ГУП \\\"Центр информационных технологий РТ\\\",l=Казань,st=16 Республика Татарстан,c=RU,1.2.840.113549.1.9.1=ca@tatar.ru,street=Петербургская 52,1.2.643.3.131.1.1=001655174024,1.2.643.100.1=1091690014712",
                                           SignedXml.XmlDsigNamespaceUrl, issuerSerialNode);
            CreateNodeWithTextIn(document, "X509SerialNumber", new BigInteger(cert.GetSerialNumber()).ToString(),
                                           SignedXml.XmlDsigNamespaceUrl, issuerSerialNode);
        }
    }
}