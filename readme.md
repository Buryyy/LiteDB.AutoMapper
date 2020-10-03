[![CodeFactor](https://www.codefactor.io/repository/github/buryyy/litedb.automapper/badge)](https://www.codefactor.io/repository/github/buryyy/litedb.automapper)
## LiteBDMapper

LiteDBMapper is an easy-to-use library for [LiteDB](https://github.com/mbdavid/LiteDB), you can easily setup a local database and store with of just 2 lines of code.

- Manages internally mapping of key and object types, you don't need to worry about it at all.
- Still access to BsonCollection to access raw bson documents.
- Store key-value pairs in batch.

## Connection string
- The connection string is supplied with the constructor when LiteBDMapper instance is being created, this is the same string as LiteDB's connection string, with same features, see - [LiteDB's connection string](https://github.com/mbdavid/LiteDB/wiki/Connection-String).

## Code examples
```cs
            //Creating instance to LiteDBMapper, it creates the collection "people" if it doesn't exist, connection string is null so it is locally stored to
            //where the application is executing from.
            var storage = new LiteDBMapper<string, IEntity>("people", null); //new LiteDBMapper<AnyKeyType, AnyValueType>

            //Storing "human jack, with age 23 to database
            storage.StoreOrUpdate("person1", new Human
            {
                Name = "Jack",
                Age = 23

            });

            //Retriving the human object from storage
            IEntity jack = storage.Get("person1");

            //Now lets print some info!
            Console.WriteLine($"Name: {jack.Name}, age: {jack.Age}"); //Prints: Name: Jack, age: 23


            //Let's create a dictionary of 100 people
            IDictionary<string, IEntity> people = new Dictionary<string, IEntity>(100);

            //Let's generate 100 people and add them to dictionary!
            for (int index = 0; index < 100; index++)
            {
                people.Add(index.ToString(), new Human //index as the number, new Human with name test and age 20 for as value.
                {
                    Name = "test" + index,
                    Age = 20
                });
            }

            //Next we can add our dictionary container directly with StoreBatch
            storage.StoreBatch(people);

            Console.WriteLine($"We have total of {people.Count} in the local storage!");
            //We could get all the values of the collection "people" that we have specified in the constructor earlier.

            IEnumerable<IEntity> enumerable = storage.Values; //Retrieves all values that we have stored earlier.

            //Now we could remove by using the same dictionary container, for deleting we only need the key.
            int removedEntities = storage.RemoveBatch(people.Keys.ToArray()); //Accepts our key type as array.

            Console.WriteLine($"We have removed a total of {removedEntities} entities.");

            //Read-only BsonCollection field is exposed, you can still access to the bson documents.
            var storedBsonDocuments = storage.BsonCollection.FindAll();

            Console.WriteLine(string.Join(Environment.NewLine, storedBsonDocuments)); //prints the jack kvp that is left in the storage.

            storage.Remove("person1"); //Jack will be removed from the storage.

            if (!storage.Contains("person1")) //Let's make sure that it doesn't exist anymore!
            {
                Console.WriteLine("Jack doesn't exist in storage."); //Actually gets printed because person1 is removed.
            }
            Console.ReadKey();
        }
        ```
