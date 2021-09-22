using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;

namespace InventoryLogin
{
    class Database
    {
        private static MongoClient dbclient = null;
        public static bool InitMongoClient()
        {
            try
            {
                if (dbclient == null)
                {
                    dbclient = new MongoClient("<YOUR MONGODB+SRV HERE>");
                }
                else
                {
                    return true;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return false;
            }

            return false;
        }

        public static string GetMongoDocument(string dbQuery, string colQuery)
        {
            return dbclient.GetDatabase(dbQuery).GetCollection<BsonDocument>(colQuery).Find(new BsonDocument()).FirstOrDefault().ToString();
        }

        public static string GetMongoDocument(string dbQuery, string colQuery, string Seek, string userPass)
        {
            try
            {
                return dbclient.GetDatabase(dbQuery).GetCollection<BsonDocument>(colQuery).Find(Builders<BsonDocument>.Filter.Eq(Seek, userPass)).FirstOrDefault().ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Entry Missing or Error! See: {e}");
            }

            return null;

        }

        public static bool GetMongoDocument(string dbQuery, string colQuery, string Seek, string userPass, bool boolUse)
        {
            try
            {
                dbclient.GetDatabase(dbQuery).GetCollection<BsonDocument>(colQuery).Find(Builders<BsonDocument>.Filter.Eq(Seek, userPass)).FirstOrDefault().ToString();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Entry Missing or Error! See: {e}");
                return false;
            }

        }

        public static String GetMongoDocumentValue(string dbQuery, string colQuery, string Seek, string value, int index)
        {
            try
            {
                return dbclient.GetDatabase(dbQuery).GetCollection<BsonDocument>(colQuery).Find(Builders<BsonDocument>.Filter.Eq(Seek, value)).FirstOrDefault().ToBsonDocument().GetValue(index).ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Entry Missing or Error!   See: {e}");
                return null;
            }
        }

        public static void PostMongoDocument(string dbQuery, string colQuery, BsonDocument postDocument)
        {
            dbclient.GetDatabase(dbQuery).GetCollection<BsonDocument>(colQuery).InsertOne(postDocument);
        }

        public static bool PingDatabase()
        {
            try
            {
                IMongoDatabase database = dbclient.GetDatabase("<REPLACE WITH DATABASE NAME>");
                database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Database offline: {e}");
                return false;
            }

        }
    }
}
