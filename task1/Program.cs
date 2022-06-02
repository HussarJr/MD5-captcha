using System.Drawing;
using System.Net;
using System.Threading.Channels;
using System;

using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Net.Http.Headers;

static string CreateMD5(string input)
{
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +
    }
}


static string parce(string md5Captcha)
{
    md5Captcha = md5Captcha.ToUpper();
    string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
    Boolean flag = false;
    for (int i = 0; i < alphabet.Length; i++)
    {
        if (flag == true)
        {
            break;
        }
        for (int j = 0; j < alphabet.Length; j++)
        {
            if (flag == true)
            {
                break;
            }
            for (int n = 0; n < alphabet.Length; n++)
            {
                if (flag == true)
                {
                    break;
                }
                for (int k = 0; k < alphabet.Length; k++)
                {

                    string Captha = alphabet[i].ToString() + alphabet[j].ToString() + alphabet[n].ToString() + alphabet[k].ToString();
                    string hashCaptha = CreateMD5(Captha);
                    if (hashCaptha.EndsWith(md5Captcha))
                    {
                        return Captha;
                        flag = true;
                        break;
                    }
                }
            }
        }
    }
    return "no result";
}

async Task GetTask()

{
    try
    {
        string url = "https://task1.tasks.rubikoid.ru/";
        string url_hash = "c3e97dd6e97fb5125688c97f36720cbe";
        string img_url = "captcha";
        string default_hash = "6457660bed88cf07f28ef3dc1a4c35e1";
        string php = ".php";
        int i = 0;
        string captha = String.Empty;
        Boolean flag = false;
        HttpClient client = new HttpClient();
        try
        {
            while (true)

            {

                var response = await client.GetAsync(url + url_hash + php);
                Console.WriteLine(i + 1 + ") " + "Status Code -- " + response.StatusCode);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseBody);
                //Console.WriteLine(responseBody.Substring(responseBody.IndexOf("<br>") + 91, 5));


                byte[] imageBytes = await client.GetByteArrayAsync(url + img_url + php); // img to txt start
                string documentsPath = "C://Users/igusa/source/repos/task1/task1/captcha_png";
                string localFilename = "captcha_" + i + ".jpg";
                string localPath = Path.Combine(documentsPath, localFilename);
                //Console.WriteLine(localPath);
                File.WriteAllBytes(localPath, imageBytes);







                Tesseract tesseract = new Tesseract();
                tesseract.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyz0123456789");
                tesseract.Init(@"C:\Users\igusa\source\repos\task1\task1\tessdata", "eng", OcrEngineMode.TesseractLstmCombined);
                tesseract.SetImage(new Image<Bgr, byte>(localPath));
                tesseract.Recognize();
                string res = tesseract.GetUTF8Text();
                tesseract.Dispose();
                string result = res[0].ToString() + res[1].ToString() + res[2].ToString() + res[3].ToString(); // end

                Console.WriteLine("OCR Result -- " + res.Trim(' ') + " -- " + result);
                captha = parce(result);
                /*
                    if (captha.Equals("no result") == true )
                    {
                        flag = false;
                        Console.WriteLine(result);
                    }
                    else
                    {
                        flag = false;
                    }
                */


                Console.WriteLine("Captha -- " + captha);
                var answer = new Dictionary<string, string>
            {
                //{"hash", default_hash },
                {"ch", captha },
                {"s", "OK" }
            };
                var content = new FormUrlEncodedContent(answer);
                response = await client.PostAsync(url + url_hash + php, content);
                responseBody = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseBody);
                string secretkey = responseBody.Substring(responseBody.IndexOf("<br>") + 91, 5);
                Console.WriteLine("Secret key -- " + secretkey);


                url_hash = CreateMD5(url_hash + secretkey).ToLower();
                //Console.WriteLine(response.Content.ReadAsStringAsync());

                /*
                var data = new StringContent(parce(result));
                response = await client.PostAsync(url, data);
                Console.WriteLine(response.StatusCode);
                response.EnsureSuccessStatusCode();

                responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                */
                i++;
            }

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("\nException Caught!");
        Console.WriteLine("Message :{0} ", e.Message);
    }

}

//string str1 = "c3e97dd6e97fb5125688c97f36720cbe";
//Console.WriteLine(CreateMD5(str1));
//Console.WriteLine(CreateMD5("B3B12534928270E0722AB71E526F4123" + "Lorem").ToLower());



await GetTask();




