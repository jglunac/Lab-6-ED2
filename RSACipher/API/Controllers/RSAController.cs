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
using Microsoft.AspNetCore.Http;

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
        [Route("rsa/keys/{p}/{q}")]

        public ActionResult GetKeys(string p, string q)
        {
            try
            {
                string basePath = _env.ContentRootPath;
                string zipPath = basePath + @"\ZipResult.zip";
                string PublicFile = basePath + @"\Temp\Public.key";
                string PrivateFile = basePath + @"\Temp\Private.key";

                int P = Convert.ToInt32(p);
                int Q = Convert.ToInt32(q);

                RSA Cypher = new RSA();

                RSAkey PublicKey = new RSAkey();
                RSAkey PrivateKey = new RSAkey();
                if (Cypher.GetKeys(P, Q, out PrivateKey, out PublicKey) == false)
                {
                    return StatusCode(500);
                }

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
                    
                }
                byte[] FileBytes = System.IO.File.ReadAllBytes(zipPath);
                if (System.IO.File.Exists(zipPath))
                {
                    System.IO.File.Delete(zipPath);
                }
                else
                {
                    return StatusCode(500);
                }
                return File(FileBytes, "application/zip", "Keys.zip");

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("rsa/{name}")]
        public async Task<ActionResult> Cipher(IFormFile file, IFormFile key, string name)
        {
            try
            {
                string basePath = _env.ContentRootPath;
                string TempFile = basePath + @"\Temp\temp.txt";
                string TempFile2 = basePath + @"\Temp\temp_key.txt";
                RSAkey Key = new RSAkey();
                byte[] FileBytes;
                if (key.FileName.Substring(key.FileName.Length-3,3) != "key")
                {
                    return StatusCode(500);
                }
                using (FileStream fs = System.IO.File.Create(TempFile2))
                {
                    await key.CopyToAsync(fs);
                }
                using (StreamReader reader = new StreamReader(TempFile2))
                {
                    string base_string = reader.ReadToEnd();
                    string[] Key_Attributes = base_string.Split("|");
                    Key.modulus = int.Parse(Key_Attributes[0]);
                    Key.power = int.Parse(Key_Attributes[1]);
                }
                using (FileStream fs = System.IO.File.Create(TempFile))
                {
                    await file.CopyToAsync(fs);
                }
                RSA Cipher = new RSA();
                if (file.FileName.Substring(file.FileName.Length - 3, 3) == "rsa")
                {
                    if (Cipher.Decipher(TempFile, out FileBytes, Key) == false)
                    {
                        return StatusCode(500);
                    }
                    return File(FileBytes, "text/plain", name + ".txt");
                }
                else
                {
                    if (Cipher.Cipher(TempFile, out FileBytes, Key) == false)
                    {
                        return StatusCode(500);
                    }
                    return File(FileBytes, "text/plain", name + ".rsa");
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
