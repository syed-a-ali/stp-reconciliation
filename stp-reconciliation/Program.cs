using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using System.Text;

namespace stp_reconciliation
{
    public class Program
    {
        static public void Main(string [] args)
        {
            var key = GetCertificate();
            var stringToSign = GetStringToSign();
            byte[] dataBytes = Encoding.GetEncoding(65001).GetBytes(stringToSign);
            var signer = new RsaDigestSigner(new Sha256Digest());
            signer.Init(true, key);
            signer.BlockUpdate(dataBytes, 0, dataBytes.Length);
            byte[] output = signer.GenerateSignature();
            var result = Convert.ToBase64String(output);
            Console.WriteLine(result);
        }

        static private string GetStringToSign()
        {
            var empresa = "GOPANGEA";
            var fechaOperacion = "";
            var stringToSign = $"|||{empresa}|{fechaOperacion}|||||||||||||||||||||||||||||||||";
            return stringToSign;
        }

        static private AsymmetricKeyParameter GetCertificate()
        {
            var keystore = "prueba.p12";
            var passphrase = "12345678";
            var alias = "GOPANGEA";
            using (var fS = File.OpenRead(keystore))
            {
                var p12Store = new Pkcs12Store(fS, passphrase.ToCharArray());
                return p12Store.GetKey(alias).Key;
            }
        }
        /*
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();*/
    }
}
