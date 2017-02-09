using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProShare
{
    public static class SecretSharing
    {
        private static Random rnd = new Random();

        /* Generates n shares with reconstruction threshold = k and secret = S 
         * S is an integer in the range of 0 to 255 */
        public static Share[] GenerateShares(byte k, byte n, byte S)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            Share[] shares = new Share[n];
            Field[] randPol = GeneratePolynomial(k, S);

            //iterate the shares
            for (byte i = 0; i < n; i++)
            {
                Field x = new Field((byte)(i + 1));
                Field y = new Field(0);

                //iterate the coefficients
                for (byte j = 0; j < k; j++)
                {
                    y += (randPol[j] * Field.pow((Field)(i + 1), j));
                }
                shares[i] = new Share(x, y);
            }

            return shares;
        }
        
        //tes!
        public static byte[][] GenerateByteShares(byte k, byte n, byte[] Secret)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            byte[][] byteShares = new byte[n][];
            for (byte i = 0; i < n; i++)
            {
                byteShares[i] = new byte[Secret.Length + 1];
            }

            //fill byteShares, array of array of share
            //byteShares[j][i] = share no. i-1 of player j
            //byteshares[j][0] is reserved to store the absissca (X value) of player j
            for (int i = 0; i < Secret.Length; i++)
            {
                Share[] CurShares = GenerateShares(k, n, Secret[i]);
                for (byte j = 0; j < n; j++)
                {
                    if (i == 0)
                    {
                        byteShares[j][0] = (byte)CurShares[j].GetX();
                    }
                    byteShares[j][i + 1] = (byte)CurShares[j].GetY();
                }
            }

            return byteShares;
        }


        public static byte[][] GenerateEncryptedByteShares(byte k, byte n, byte[] Secret, byte[] key)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            byte[][] byteShares = new byte[n][];
            for (byte i = 0; i < n; i++)
            {
                byteShares[i] = new byte[Secret.Length + 1];
            }
            
            //fill byteShares, array of array of share
            //byteShares[j][i] = share no. i-1 of player j
            //byteshares[j][0] is reserved to store the absissca (X value) of player j
            for (int i = 0; i < Secret.Length; i++)
            {
                Share[] CurShares = GenerateShares(k, n, Secret[i]);
                byte[] curBytes = new byte[Secret.Length + 1];

                for (byte j = 0; j < n; j++)
                {
                    if (i == 0)
                    {
                        byteShares[j][0] = (byte)CurShares[j].GetX();
                        //curBytes[0] = (byte)CurShares[j].GetX();
                    }
                    byteShares[j][i + 1] = (byte)CurShares[j].GetY();
                    //TEST LAMA
                    //curBytes[i + 1] = (byte)CurShares[j].GetY();
                    //byteShares[j] = AESEncryptBytes(curBytes, key);
                }
            }

            //Writing real shares
            for (int i = 0; i < byteShares.Length; i++)
            {
                File.WriteAllBytes(i + "-real.txt", byteShares[i]);
            }

            //TEST BARU
            byte[][] encryptedByteMatrix = new byte[n][];
            for (byte i = 0; i < n; i++)
            {
                encryptedByteMatrix[i] = new byte[Secret.Length + 1];
            }

            for (int i=0; i<n; i++)
            {
                encryptedByteMatrix[i] = AESEncryptBytes(byteShares[i], key);
            }
            //return byteShares;
            //TEST
            return encryptedByteMatrix;
        }

        /* Generates n files as shares (their locations are the return value) with reconstruction threshold = k and secret = S 
         * SLocation is the location to the file that is given as the secret. The file's size must not exceed 2^32 bytes (~4.2 GB) */
        //TODO : Handle file I/O exceptions
        //TODO : Reformat share names
        public static string[] GenerateFileShares(byte k, byte n, string SLocation)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            string[] FileShareNames = new string[n];
            try
            {
                using (FileStream fs = new FileStream(SLocation, FileMode.Open, FileAccess.Read))
                {
                    byte[] FileArr = new byte[fs.Length];
                    int bytesLeft = (int)fs.Length;
                    int bytesRead = 0;
                    while (bytesLeft > 0)
                    {
                        int res = fs.Read(FileArr, bytesRead, bytesLeft);
                        if (res == 0)
                            break;
                        bytesRead += res;
                        bytesLeft -= res;
                    }

                    byte[][] byteShares = new byte[n][];
                    for (byte i = 0; i < n; i++)
                    {
                        byteShares[i] = new byte[FileArr.Length + 1];
                    }

                    //fill byteShares, array of array of share
                    //byteShares[j][i] = share no. i-1 of player j
                    //byteshares[j][0] is reserved to store the absissca (X value) of player j
                    for (int i = 0; i < FileArr.Length; i++)
                    {
                        Share[] CurShares = GenerateShares(k, n, FileArr[i]);
                        for (byte j = 0; j < n; j++)
                        {
                            if (i == 0)
                            {
                                byteShares[j][0] = (byte)CurShares[j].GetX();
                            }
                            byteShares[j][i + 1] = (byte)CurShares[j].GetY();
                        }
                    }

                    //writing share files
                    for (byte i = 0; i < n; i++)
                    {
                        string ShareFileName = "output" + (i + 1) + ".share";
                        using (FileStream fsWrite = new FileStream(ShareFileName, FileMode.Create, FileAccess.Write))
                        {
                            fsWrite.Write(byteShares[i], 0, byteShares[i].Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return FileShareNames;
        }

        /* Reconstructs secret from first "k" shares from array of share (shares) 
         * using Lagrange Polynomial */
        public static Field ReconstructSecret(Share[] shares, byte k)
        {
            if (shares.Length == 0 || k == 0)
            {
                throw new System.ArgumentException("Shares cannot be empty and k cannot be 0", "shares and k");
            }
            if (shares.Length < k)
            {
                throw new System.ArgumentException("Shares size cannot be less than k", "shares and k");
            }

            Field S = new Field(0);
            for (byte i = 0; i < k; i++)
            {
                Field CurShare = shares[i].GetY();
                for (byte j = 0; j < k; j++)
                {
                    if (j != i)
                    {
                        CurShare *= (shares[j].GetX() / (shares[j].GetX() - shares[i].GetX()));
                    }
                }
                S += CurShare;
            }
            return S;
        }

        public static byte[] ReconstructByteSecret(byte[][] byteShares, byte k)
        {
            Share[][] shares = new Share[k][];
            for(int i=0; i<shares.Length; i++)
            {
                byte X = (byte)i; //default value
                Share[] curShares = new Share[byteShares[i].Length - 1];
                for(int j=0; j<byteShares[i].Length; j++)
                {
                    //getting the x value in front of the file
                    if (j == 0)
                    {
                        X = byteShares[i][j];
                    }
                    //convert the rest of file to Share
                    else
                    {
                        curShares[j - 1] = new Share((Field)X, (Field)byteShares[i][j]);
                    }
                }
                shares[i] = curShares;
            }

            //reconstruction process
            Field[] Secret = new Field[shares[0].Length];
            byte[] secretBytes = new byte[Secret.Length];
            for (int i = 0; i < shares[0].Length; i++)
            {
                Share[] CurShares = new Share[k];
                for (int j = 0; j < k; j++)
                {
                    CurShares[j] = shares[j][i];
                }
                Secret[i] = ReconstructSecret(CurShares, k);
                secretBytes[i] = (byte)Secret[i];
            }

            return secretBytes;
        }

        /* Reconstructs secret from first "k" share files from the array of file locations (ShareFilesNames) 
         * then writes the secret into SecretFileLocation */
        //TODO : Handle file I/O exceptions
        //TODO : Filter only .share files?
        public static void ReconstructFileSecret(string[] ShareFilesLocations, byte k, string SecretFileLocation)
        {
            if (ShareFilesLocations.Length == 0 || k == 0)
            {
                throw new System.ArgumentException("Share file locations cannot be empty and k cannot be 0", "ShareFilesNames and k");
            }
            if (ShareFilesLocations.Length < k)
            {
                throw new System.ArgumentException("The number of Share files cannot be less than k", "ShareFilesNames and k");
            }

            try
            {
                Share[][] Shares = new Share[k][];

                //reading file to shares
                for (int i = 0; i < k; i++)
                {
                    using (FileStream fs = new FileStream(ShareFilesLocations[i], FileMode.Open, FileAccess.Read))
                    {
                        byte X = (byte)i; //default value
                        Share[] CurShares = new Share[fs.Length - 1];

                        for (int j = 0; j < fs.Length; j++)
                        {
                            //getting the x value in front of the file
                            if (j == 0)
                            {
                                X = (byte)fs.ReadByte();
                            }
                            //convert the rest of file to Share
                            else
                            {
                                CurShares[j - 1] = new Share((Field)X, (Field)fs.ReadByte());
                            }
                        }
                        Shares[i] = CurShares;
                    }
                }

                //reconstruction process
                Field[] Secret = new Field[Shares[0].Length];
                byte[] SecretBytes = new byte[Secret.Length];
                for (int i = 0; i < Shares[0].Length; i++)
                {
                    Share[] CurShares = new Share[k];
                    for (int j = 0; j < k; j++)
                    {
                        CurShares[j] = Shares[j][i];
                    }
                    Secret[i] = ReconstructSecret(CurShares, k);
                    SecretBytes[i] = (byte)Secret[i];
                }

                //writing secret to file
                using (FileStream fsWrite = new FileStream(SecretFileLocation, FileMode.Create, FileAccess.Write))
                {
                    fsWrite.Write(SecretBytes, 0, SecretBytes.Length);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* Reconstructs secret from first "k" share files from the array of file locations (ShareFilesNames) 
         * then writes the secret into SecretFileLocation */
        //TODO : Handle file I/O exceptions
        //TODO : Filter only .share files?
        public static void ReconstructFileSecret(string[] ShareFilesLocations, byte k, string SecretFileLocation, byte[] key)
        {
            if (ShareFilesLocations.Length == 0 || k == 0)
            {
                throw new System.ArgumentException("Share file locations cannot be empty and k cannot be 0", "ShareFilesNames and k");
            }
            if (ShareFilesLocations.Length < k)
            {
                throw new System.ArgumentException("The number of Share files cannot be less than k", "ShareFilesNames and k");
            }

            try
            {
                Share[][] Shares = new Share[k][];

                //reading file to shares
                for (int i = 0; i < k; i++)
                {
                    using (FileStream fs = new FileStream(ShareFilesLocations[i], FileMode.Open, FileAccess.Read))
                    {
                        byte X = (byte)i; //default value
                        Share[] CurShares = new Share[fs.Length - 1];

                        for (int j = 0; j < fs.Length; j++)
                        {
                            //getting the x value in front of the file
                            if (j == 0)
                            {
                                X = (byte)fs.ReadByte();
                            }
                            //convert the rest of file to Share
                            else
                            {
                                CurShares[j - 1] = new Share((Field)X, (Field)fs.ReadByte());
                            }
                        }
                        Shares[i] = CurShares;
                    }
                }

                //reconstruction process
                Field[] Secret = new Field[Shares[0].Length];
                byte[] SecretBytes = new byte[Secret.Length];
                for (int i = 0; i < Shares[0].Length; i++)
                {
                    Share[] CurShares = new Share[k];
                    for (int j = 0; j < k; j++)
                    {
                        CurShares[j] = Shares[j][i];
                    }
                    Secret[i] = ReconstructSecret(CurShares, k);
                    SecretBytes[i] = (byte)Secret[i];
                }
                SecretBytes = AESDecryptBytes(SecretBytes, key);
                //writing secret to file
                using (FileStream fsWrite = new FileStream(SecretFileLocation, FileMode.Create, FileAccess.Write))
                {
                    fsWrite.Write(SecretBytes, 0, SecretBytes.Length);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /* Generates coefficients of a random polynomial with degree = k-1 and a0 = S */
        private static Field[] GeneratePolynomial(byte k, byte S)
        {
            if (k == 0)
            {
                throw new System.ArgumentException("Length cannot be 0", "k");
            }
            Field[] fields = new Field[k];
            fields[0] = new Field(S);

            for (byte i = 1; i < k; i++)
            {
                byte current = (byte)rnd.Next(1, Field.Order);
                Field f = new Field(current);
                fields[i] = f;
            }
            return fields;
        }

        /* Generates subshares from a player. XArr is array of abscissa (x) of all players and k is the threshold number */
        private static Field[] GenerateSubshares(Field[] XArr, byte k)
        {
            if ((XArr.Length == 0) || (k == 0))
            {
                throw new System.ArgumentException("Array of players cannot be empty and k cannot be 0", "XArr and k");
            }
            if (XArr.Length < k)
            {
                throw new System.ArgumentException("Array of players' size cannot be less than k", "XArr and k");
            }
            Field[] RandPol = GeneratePolynomial(k, 0);
            Field[] subshares = new Field[XArr.Length];
            for (byte i = 0; i < subshares.Length; i++)
            {
                Field curTotal = new Field(0);
                for (byte j = 0; j < RandPol.Length; j++)
                {
                    curTotal += RandPol[j] * Field.pow(XArr[i], j);
                }
                subshares[i] = curTotal;
            }
            return subshares;
        }

        //generates list of subshares from one player
        public static byte[] GenerateByteSubshares(byte k, byte n)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            //Generates array of absissca (x) from 1 to n
            Field[] XArr = new Field[n];
            for(byte i=0; i<n; i++)
            {
                XArr[i] = (Field)(i + 1);
            }

            //Generates byte[] of subshare
            Field[] subshare = GenerateSubshares(XArr, k);
            byte[] subshareBytes = new byte[subshare.Length];
            for(int i=0; i<subshareBytes.Length; i++)
            {
                subshareBytes[i] = (byte)subshare[i];
            }

            return subshareBytes;
        }

        public static byte[] GenerateEncryptedByteSubshares(byte k, byte n, byte[] key)
        {
            if (k == 0 || n == 0)
            {
                throw new System.ArgumentException("k and n cannot be 0", "k and n");
            }
            if (k > n)
            {
                throw new System.ArgumentException("k must be less or equal than n", "k and n");
            }

            //Generates array of absissca (x) from 1 to n
            Field[] XArr = new Field[n];
            for (byte i = 0; i < n; i++)
            {
                XArr[i] = (Field)(i + 1);
            }

            //Generates byte[] of subshare
            Field[] subshare = GenerateSubshares(XArr, k);
            byte[] subshareBytes = new byte[subshare.Length];
            for (int i = 0; i < subshareBytes.Length; i++)
            {
                //subshareBytes[i] = (byte)subshare[i];
                //TEST ENCRYPT
                byte curByte = (byte) subshare[i];
                byte[] curBytes = { curByte };
                byte[] curEncryptedBytes = AESEncryptBytes(curBytes, key);
                byte curEncryptedByte = curEncryptedBytes[0];
                subshareBytes[i] = curEncryptedByte;
            }

            return subshareBytes;
        }

        /* Generates new share for a player according to his/her old share (CurShare) and list of subshare (subshares) from other players */
        private static Share GenerateNewShare(Share CurShare, Field[] subshares)
        {
            if (subshares.Length == 0)
            {
                throw new System.ArgumentException("Array subshares cannot be empty", "subshares");
            }
            Field NewX = CurShare.GetX();
            Field NewY = CurShare.GetY();
            for (byte i = 0; i < subshares.Length; i++)
            {
                NewY += subshares[i];
            }
            Share NewShare = new Share(NewX, NewY);
            return NewShare;
        }

        /* Generates new file share according to his/her old share file in OldShareLocation and a list of subshare (subshares) from other players
         * writes the new file share in NewShareLocation */
        //kalo perlu, bikin method baru yg nerima dan output byte[]
        public static void GenerateNewFileShare(string OldShareLocation, Field[] subshares, string NewShareLocation)
        {
            if (subshares.Length == 0)
            {
                throw new System.ArgumentException("Array subshares cannot be empty", "subshares");
            }

            try
            {
                using (FileStream fs = new FileStream(OldShareLocation, FileMode.Open, FileAccess.Read))
                {
                    //reads old file to shares
                    Share[] Shares = new Share[fs.Length - 1];
                    byte X = 0;
                    for (int i = 0; i < fs.Length; i++)
                    {
                        if (i == 0)
                        {
                            X = (byte)fs.ReadByte();
                        }
                        else
                        {
                            Shares[i - 1] = new Share((Field)X, (Field)fs.ReadByte());
                        }
                    }

                    //generating new shares
                    Share[] NewShares = new Share[Shares.Length];
                    for (int i = 0; i < NewShares.Length; i++)
                    {
                        NewShares[i] = GenerateNewShare(Shares[i], subshares);
                    }

                    //writing new shares into file
                    byte[] NewShareBytes = new byte[NewShares.Length + 1];
                    for (int i = 0; i < NewShareBytes.Length; i++)
                    {
                        if (i == 0)
                        {
                            NewShareBytes[i] = (byte)NewShares[0].GetX();
                        }
                        else
                        {
                            NewShareBytes[i] = (byte)NewShares[i - 1].GetY();
                        }
                    }

                    using (FileStream fsWrite = new FileStream(NewShareLocation, FileMode.Create, FileAccess.Write))
                    {
                        fsWrite.Write(NewShareBytes, 0, NewShareBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static byte[] GenerateNewShareBytes(byte[] oldShareBytes, byte[] subshareBytes)
        {
            if (subshareBytes.Length == 0 || oldShareBytes.Length == 0)
            {
                throw new System.ArgumentException("Array subshares & oldsharebytes cannot be empty", "subshares");
            }

            Share[] oldShares = new Share[oldShareBytes.Length - 1];
            byte X = 0;
            for (int i=0; i<oldShareBytes.Length; i++)
            {
                if (i == 0)
                {
                    X = oldShareBytes[i];
                }
                else
                {
                    oldShares[i - 1] = new Share((Field)X, (Field)oldShareBytes[i]);
                }
            }

            //generating new shares
            Field[] subshares = new Field[subshareBytes.Length];
            for (int i=0; i<subshares.Length; i++)
            {
                subshares[i] = (Field)subshareBytes[i];
            }

            Share[] newShares = new Share[oldShares.Length];
            for (int i = 0; i < newShares.Length; i++)
            {
                newShares[i] = GenerateNewShare(oldShares[i], subshares);
            }

            byte[] newShareBytes = new byte[newShares.Length + 1];
            for (int i = 0; i < newShareBytes.Length; i++)
            {
                if (i == 0)
                {
                    newShareBytes[i] = (byte)newShares[0].GetX();
                }
                else
                {
                    newShareBytes[i] = (byte)newShares[i - 1].GetY();
                }
            }

            return newShareBytes;
        }

        //BELUM SELESAI! DONT USE!
        public static byte[] GenerateNewDecryptedShareBytes(byte[] oldShareBytes, byte[] subshareBytes, byte[] key)
        {
            if (subshareBytes.Length == 0 || oldShareBytes.Length == 0)
            {
                throw new System.ArgumentException("Array subshares & oldsharebytes cannot be empty", "subshares");
            }

            Share[] oldShares = new Share[oldShareBytes.Length - 1];
            byte X = 0;
            for (int i = 0; i < oldShareBytes.Length; i++)
            {
                if (i == 0)
                {
                    X = oldShareBytes[i];
                }
                else
                {
                    oldShares[i - 1] = new Share((Field)X, (Field)oldShareBytes[i]);
                }
            }

            //generating new shares
            Field[] subshares = new Field[subshareBytes.Length];
            for (int i = 0; i < subshares.Length; i++)
            {
                subshares[i] = (Field)subshareBytes[i];
            }

            Share[] newShares = new Share[oldShares.Length];
            for (int i = 0; i < newShares.Length; i++)
            {
                newShares[i] = GenerateNewShare(oldShares[i], subshares);
            }

            byte[] newShareBytes = new byte[newShares.Length + 1];
            for (int i = 0; i < newShareBytes.Length; i++)
            {
                if (i == 0)
                {
                    newShareBytes[i] = (byte)newShares[0].GetX();
                }
                else
                {
                    newShareBytes[i] = (byte)newShares[i - 1].GetY();
                }
            }

            return newShareBytes;
        }

       
        private static byte[] AESEncryptBytes(byte[] clearBytes, byte[] passBytes)
        {
            byte[] saltBytes = Encoding.ASCII.GetBytes("12345678");
            byte[] encryptedBytes = null;

            // create a key from the password and salt, use 32K iterations – see note
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 8192);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(),
          CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static byte[] AESDecryptBytes(byte[] cryptBytes, byte[] passBytes)
        {
            byte[] saltBytes = Encoding.ASCII.GetBytes("12345678");
            byte[] clearBytes = null;

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 8192);

            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }

        /*private static byte[] AESEncryptBytes(byte[] inputBytes, byte[] key)
        {
            string IV = "1234567890123456";
            AesCryptoServiceProvider dataencrypt = new AesCryptoServiceProvider();
            //Block size : Gets or sets the block size, in bits, of the cryptographic operation.  
            dataencrypt.BlockSize = 128;
            //KeySize: Gets or sets the size, in bits, of the secret key  
            dataencrypt.KeySize = 128;
            //Key: Gets or sets the symmetric key that is used for encryption and decryption.  
            dataencrypt.Key = key;
            //IV : Gets or sets the initialization vector (IV) for the symmetric algorithm  
            dataencrypt.IV = System.Text.Encoding.ASCII.GetBytes(IV);
            //Padding: Gets or sets the padding mode used in the symmetric algorithm  
            dataencrypt.Padding = PaddingMode.PKCS7;
            //Mode: Gets or sets the mode for operation of the symmetric algorithm  
            dataencrypt.Mode = CipherMode.CBC;
            //Creates a symmetric AES encryptor object using the current key and initialization vector (IV).  
            ICryptoTransform crypto1 = dataencrypt.CreateEncryptor(dataencrypt.Key, dataencrypt.IV);
            //TransformFinalBlock is a special function for transforming the last block or a partial block in the stream.   
            //It returns a new array that contains the remaining transformed bytes. A new array is returned, because the amount of   
            //information returned at the end might be larger than a single block when padding is added.  
            byte[] encrypteddata = crypto1.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            crypto1.Dispose();
            //return the encrypted data  
            return encrypteddata;
        }

        private static byte[] AESDecryptBytes(byte[] inputBytes, byte[] key)
        {
            string IV = "1234567890123456";
            AesCryptoServiceProvider keydecrypt = new AesCryptoServiceProvider();
            keydecrypt.BlockSize = 128;
            keydecrypt.KeySize = 128;
            keydecrypt.Key = key;
            keydecrypt.IV = System.Text.Encoding.ASCII.GetBytes(IV);
            keydecrypt.Padding = PaddingMode.PKCS7;
            keydecrypt.Mode = CipherMode.CBC;
            ICryptoTransform crypto1 = keydecrypt.CreateDecryptor(keydecrypt.Key, keydecrypt.IV);

            byte[] returnbytearray = crypto1.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            crypto1.Dispose();
            return returnbytearray;
        }*/
    }
}
