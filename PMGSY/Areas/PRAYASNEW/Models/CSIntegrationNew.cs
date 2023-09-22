//Please import the following namespaces.
//================================================
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;


/// <summary>
/// Summary description for CSIntegration
/// </summary>
public class CSIntegrationNew
{
    // Method to Get MD5 Hash Value of Input String Value.
    public string GetMD5Hasher(string strtext)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] bytHashedData = null;
        bytHashedData = md5.ComputeHash(Encoding.UTF8.GetBytes(strtext));
        StringBuilder hex = new StringBuilder(bytHashedData.Length * 2);
        byte b = 0;

        foreach (byte b_loopVariable in bytHashedData)
        {
            b = b_loopVariable;
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }

    // Method to Read Key File saved in Website Folder and Encrypt your Data with this Key.
    public string Encrypt(string textToEncrypt, string FilePath)
    {
        // Dim rijndaelCipher As AesCryptoServiceProvider = New AesCryptoServiceProvider()
        // rijndaelCipher.Mode = CipherMode.CTS

        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;

        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 0x80;
        rijndaelCipher.BlockSize = 0x80;
        byte[] pwdBytes = GetFileBytes(FilePath);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
            len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;
        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    // Method to Encrypt your Data with passed Key Bytes.
    public string EncryptB(string textToEncrypt, byte[] FileByte)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 0x80;
        rijndaelCipher.BlockSize = 0x80;
        byte[] pwdBytes = FileByte;
        byte[] keyBytes = new byte[16] ;
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
            len = keyBytes.Length;
        Array.Copy(pwdBytes, keyBytes, len);
        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;
        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    // Method to Read Key Bytes from File saved in Website Folder.
    public byte[] GetFileBytes(string filePath)
    {
        byte[] buffer;
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        try
        {
            int length = System.Convert.ToInt32(fileStream.Length);
            buffer = new byte[length - 1 + 1 - 1 + 1];
            int count;
            int sum = 0;

            while (((count = fileStream.Read(buffer, sum, length - sum)) > 0))
            {
                sum += count;
            }
        }
        finally
        {
            fileStream.Close();
        }

        return buffer;
    }
}