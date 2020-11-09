using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;
using RSACipher;
using System.IO;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RSAController : ControllerBase
    {
        private IWebHostEnvironment _env;
        public RSAController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("/rsa/keys/{p}/{q}")]

        public ActionResult GetKeys(string p, string q)
        {
            try
            {
                string basePath = _env.ContentRootPath;
                string zipPath = basePath + @"\ZipResult.zip";
                string PublicFile = basePath + @"\Temp\Public.key";
                string PrivateFile = basePath + @"\Temp\Private.key";

                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }

                int P = Convert.ToInt32(p);
                int Q = Convert.ToInt32(q);

                RSA Cypher = new RSA();

                RSAkey PublicKey = new RSAkey();
                RSAkey PrivateKey = new RSAkey();

                //Cypher.GetKeys(P, Q, out PrivateKey, out PublicKey);
                PublicKey.modulus = 6;
                PublicKey.power = 9;
                PrivateKey.modulus = 27;
                PrivateKey.power = 48;
                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    using (StreamWriter writter = new StreamWriter(PublicFile))
                    {
                        writter.Write(PublicKey.modulus);
                        writter.Write("|");
                        writter.Write(PublicKey.power);
                    }
                    archive.CreateEntryFromFile(PublicFile, "Public.key");
                    using (StreamWriter writter = new StreamWriter(PrivateFile))
                    {
                        writter.Write(PrivateKey.modulus);
                        writter.Write("|");
                        writter.Write(PrivateKey.power);
                    }
                    archive.CreateEntryFromFile(PrivateFile, "Private.key");
                    System.IO.File.Delete(PublicFile);
                    System.IO.File.Delete(PrivateFile);
                    return Ok(archive);
                }

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
