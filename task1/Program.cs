using System.Net;



static string CreateMD5(string input)
{
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +

        // Convert the byte array to hexadecimal string prior to .NET 5
        // StringBuilder sb = new System.Text.StringBuilder();
        // for (int i = 0; i < hashBytes.Length; i++)
        // {
        //     sb.Append(hashBytes[i].ToString("X2"));
        // }
        // return sb.ToString();
    }
}



static string parce(string md5Captcha)
{
    md5Captcha = md5Captcha.ToUpper();
    string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
    for (int i = 0; i < alphabet.Length; i++)
    {
        for (int j = 0; j < alphabet.Length; j++)
        {
            for (int n = 0; n < alphabet.Length; n++)
            {
                for (int k = 0; k < alphabet.Length; k++)
                {
                    string Captha = alphabet[i].ToString() + alphabet[j].ToString() + alphabet[n].ToString() + alphabet[k].ToString();
                    string hashCaptha = CreateMD5(Captha);
                    if (hashCaptha.EndsWith(md5Captcha))
                    {
                        return Captha + " -- " + hashCaptha;
                    }
                }
            }
        }
    }
    return md5Captcha + "-- no result";
}

string str1 = "3583";
string str2 = "AB6F";
Console.WriteLine(parce(str1.ToUpper()));
Console.WriteLine(parce(str2));

Console.WriteLine(CreateMD5("c3e97dd6e97fb5125688c97f36720cbe" + "Lorem").ToLower());