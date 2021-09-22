using MongoDB.Bson;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace InventoryLogin
{
    class Authentication
    {
        static public bool loggedIn = false;
        static public string loginUserFirstName = null;
        static public string loginUserFlags = null;
        static private string authPass = null;
        static private string authUser = null;

        private static string Encrypt(string value)
        {
            using (SHA512CryptoServiceProvider sHA = new SHA512CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = sHA.ComputeHash(utf8.GetBytes(value));
                return Convert.ToBase64String(data);
            }
        }

        public static void Login(string userText, string passText)
        {
            if (string.IsNullOrEmpty(passText))
            {
                MessageBox.Show("Please enter your password.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(userText))
            {
                MessageBox.Show("Please enter your username.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentDate = DateTime.Now.ToString().Substring(0, 17);
            authPass = Encrypt(Encrypt(passText) + currentDate + Encrypt(userText));
            authUser = Encrypt(userText);

            Console.WriteLine(authPass);

            switch (ReqAuth(currentDate, authPass, authUser))
            {
                case true:
                    loggedIn = true;
                    loginUserFlags = Database.GetMongoDocumentValue("userLogin", "userData", "authUser", authUser, 3);
                    loginUserFirstName = Database.GetMongoDocumentValue("userLogin", "userData", "authUser", authUser, 4);
                    break;
                case false:
                    MessageBox.Show("User/Password Incorrect.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            };


            authPass = null;
            authUser = null;
        }

        public static void Register(string userText, string passText)
        {
            if (string.IsNullOrEmpty(passText))
            {
                MessageBox.Show("Please enter your password.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(userText))
            {
                MessageBox.Show("Please enter your username.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (userText.Length < 5 || userText.Length > 50)
            {
                MessageBox.Show("Username must be at least 5 characters, no more than 50.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (passText.Length < 8 || passText.Length > 50)
            {
                MessageBox.Show("Password must be at least 8 characters, no more than 50.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            authPass = Encrypt(passText);
            authUser = Encrypt(userText);

            if (Database.GetMongoDocument("userLogin", "userData", "authUser", authUser, true))
            {
                MessageBox.Show("There is already a username registered with that account.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                authPass = null;
                authUser = null;
                return;
            }
            else
            {
                Database.PostMongoDocument("userLogin", "userData", new BsonDocument { { "authUser", authUser }, { "authPass", authPass }, { "UserFlags", -999999 } });

                MessageBox.Show("New user created!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                authPass = null;
                authUser = null;
                return;
            }
        }

        private static bool ReqAuth(string current, string authPass, string authUser)
        {
            if (Database.GetMongoDocument("userLogin", "userData", "authUser", authUser, true))
            {
                string authPassDB = Database.GetMongoDocumentValue("userLogin", "userData", "authUser", authUser, 2);
                string authSalt = Encrypt(authPassDB + current + authUser);
                Console.WriteLine(authSalt);
                if (authSalt == authPass)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
