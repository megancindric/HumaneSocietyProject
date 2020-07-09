using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No clients have a ClientId that matches the Client passed in.");
                UserInterface.DisplayMessage("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the Create(insert)Read(select)UpdateDelete operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            //Switch case for all CRUD operations
            switch(crudOperation)
            {
                case "Create":
                    AddEmployee(employee);
                    break;

                case "Read":
                    GetEmployeeByID(employee);
                    break;

                case "Update":
                    UpdateEmployee(employee);
                    break;

                case "Delete":
                    RemoveEmployee(employee);
                    break;

                default:
                    UserInterface.DisplayMessage("Not a valid query operation.  Please try your query again");
                    break;
            }
             
        }

        //Create employee
        internal static void AddEmployee(Employee employee)
        {
            //Copied from Client update - let's double check this!
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }
        //Read employee
        internal static Employee GetEmployeeByID(Employee employee)
        {
            Employee employeeByID = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
            return employeeByID;
        }
        //Update employee
        internal static void UpdateEmployee(Employee employee)
        {
            Employee employeeFromDb = null;

            try
            {
                employeeFromDb = db.Employees.Where(c => c.EmployeeId == employee.EmployeeId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No employees have an EmployeeId that matches the Employee passed in.");
                UserInterface.DisplayMessage("No update have been made.");
                return;
            }

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            employeeFromDb.FirstName = employee.FirstName;
            employeeFromDb.LastName = employee.LastName;
            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;
            employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
            employeeFromDb.Email = employee.Email;

            db.SubmitChanges();
        }
        //Delete employee
        internal static void RemoveEmployee(Employee employee)
        {
            db.Employees.DeleteOnSubmit(employee);
            db.SubmitChanges();
        }
        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animalByID = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animalByID;  
        }

        internal static void UpdateAnimal(Animal updatedAnimal)
        {
            Animal animalFromDb = null;

            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == updatedAnimal.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }

            animalFromDb.Name = updatedAnimal.Name;
            animalFromDb.Weight = updatedAnimal.Weight;
            animalFromDb.Age = updatedAnimal.Age;
            animalFromDb.Demeanor = updatedAnimal.Demeanor;
            animalFromDb.KidFriendly = updatedAnimal.KidFriendly;
            animalFromDb.PetFriendly = updatedAnimal.PetFriendly;
            animalFromDb.Gender = updatedAnimal.Gender;
            animalFromDb.AdoptionStatus = updatedAnimal.AdoptionStatus;
            animalFromDb.CategoryId = updatedAnimal.CategoryId;
            animalFromDb.DietPlanId = updatedAnimal.DietPlanId;
            animalFromDb.EmployeeId = updatedAnimal.EmployeeId;

            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
            
            //Clarification on this method?
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> searchCriteria) // parameter(s)? **FOLLOW UP W/ SENIOR DEV ABOUT THIS ONE**
        {
            var allAnimals = db.Animals;
           
            //Grab all animals from DB & store in var
            foreach (var item in searchCriteria)
            {
                switch(item.Key)
                {
                    case 1: //Category
                      allAnimals.Where(c => c.Category.Name == item.Value).FirstOrDefault();
                      break;

                    case 2: //Name
                       allAnimals.Where(c => c.Name == item.Value).FirstOrDefault();
                       break;

                    case 3: //Age
                        int ageInInt = Int32.Parse(item.Value);
                        allAnimals.Where(c => c.Age == ageInInt).FirstOrDefault();
                        break;

                    case 4: //Demeanor
                        allAnimals.Where(c => c.Demeanor == item.Value).FirstOrDefault();
                        break;

                    case 5: //Kid Friendly
                        bool kidFriendly = UserInterface.YesNoToBool(item.Value);
                        allAnimals.Where(c => c.KidFriendly == kidFriendly).FirstOrDefault();
                        break;

                    case 6: //Pet Friendly
                        bool petFriendly = UserInterface.YesNoToBool(item.Value);
                        allAnimals.Where(c => c.PetFriendly == petFriendly).FirstOrDefault();
                        break;

                    case 7: //Weight
                        int weightInInt = Int32.Parse(item.Value);
                        allAnimals.Where(c => c.Weight == weightInInt).FirstOrDefault();
                        break;

                    case 8: //ID
                        int idInInt = Int32.Parse(item.Value);
                        allAnimals.Where(c => c.AnimalId == idInInt).FirstOrDefault();
                        break;

                    default:
                        UserInterface.DisplayMessage("Improper search parameters passed in.");
                        UserInterface.DisplayMessage("Search was unsuccessful. Please try your search again");
                        break;
                }
            }

            return allAnimals;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category correctCategory = db.Categories.Where(a => a.Name == categoryName).FirstOrDefault();
            return correctCategory.CategoryId;
        }

        internal static Room GetRoom(int animalId)
        {
            Room roomWithAnimal = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            return roomWithAnimal;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlanById = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();
            return dietPlanById.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            //Creation of Adoption (linking AnimalId and ClientId).  Will set AdoptionStatus to Pending, PaymentCollect to FALSE, Can hard code PaymentFee
            Adoption animalAdoption = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId && a.ClientId == client.ClientId).FirstOrDefault();
            
            

            Animal animalFromDB = db.Animals.Where(b => b.AnimalId == animal.AnimalId).FirstOrDefault();
            animalFromDB.AdoptionStatus = "Pending";
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()  //Instead of yield return, we will create a list of Adoptions where = "Pending", then return this list
        {
            var animalsPendingAdoption = db.Animals.Where(p => p.AdoptionStatus == "Pending").Select(a => a.AnimalId);
            var listOfAdoptions = db.Adoptions;
            var finalList;
            //We now have a collection of all adoptions, and a collection of the ID's we need to pull from
            //Can we join animalId's with list of adoptions, then simply select just "Adoptions" fromt that collection?

            foreach (var animalID in animalsPendingAdoption)
            {
                finalList.Add(listOfAdoptions.Where(a => a.AnimalId == animalID));
            }
           
            return finalList;
                                                                                                                                       //START HERE THURSDAY
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            //Search DB for this particular adoption
            Adoption adoptionFromDb = null;
            Animal animalFromDb = null;
            try
            {
                adoptionFromDb = db.Adoptions.Where(c => c.AnimalId == adoption.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No adoption matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == adoption.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            //If isAdopted --> payment has been collected, adoption is approved, adoption status = "Adopted"
            //Otherwise we can mark as not adopted (will keep record of this adoption in DB)
            if (isAdopted)
            {
                animalFromDb.AdoptionStatus = "Adopted";
                //Will need to change AdoptionStatus at that animal to Adopted
            }

            else
            {
                animalFromDb.AdoptionStatus = "Not Adopted";
                //Will need to change AdoptionStatus to NotAdopted
            }
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            //Will change Animal.Adoption Status to notadopted
           
            Adoption adoptionToRemove = null;
            try
            {
                adoptionToRemove = db.Adoptions.Where(c => c.AnimalId == animalId && c.ClientId == clientId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No record of adoption matches the values passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            Animal animalFromDb = null;
            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == animalId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            animalFromDb.AdoptionStatus = "Not Adopted";
            db.Adoptions.DeleteOnSubmit(adoptionToRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            Animal animalFromDb = null;

            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException e)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return null;
            }

            var listOfShots = db.AnimalShots.Where(s => s.AnimalId == animalFromDb.AnimalId);
            return listOfShots;

        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}