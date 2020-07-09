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
            catch(InvalidOperationException)
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
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No employees have an EmployeeId that matches the Employee passed in.");
                UserInterface.DisplayMessage("No update have been made.");
                return;
            }

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

        internal static void UpdateAnimal(int animalID,Dictionary<int,string> updates)
        {
            Animal animalFromDb = null;
            var allCategories = db.Categories;

            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == animalID).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            foreach (var item in updates)
            {
                switch (item.Key)
                {
                    case 1: //Category
                        Category updatedCategory = allCategories.Where(a => a.Name == item.Value).FirstOrDefault();
                        animalFromDb.Category = updatedCategory;
                        break;

                    case 2: //Name
                        animalFromDb.Name = item.Value;
                        break;

                    case 3: //Age
                        int ageInInt = Int32.Parse(item.Value);
                        animalFromDb.Age = ageInInt;
                        break;

                    case 4: //Demeanor
                        animalFromDb.Demeanor = item.Value;
                        break;

                    case 5: //Kid Friendly
                        bool kidFriendly = UserInterface.YesNoToBool(item.Value);
                        animalFromDb.KidFriendly = kidFriendly;
                        break;

                    case 6: //Pet Friendly
                        bool petFriendly = UserInterface.YesNoToBool(item.Value);
                        animalFromDb.PetFriendly = petFriendly;
                        break;

                    case 7: //Weight
                        int weightInInt = Int32.Parse(item.Value);
                        animalFromDb.Weight = weightInInt;
                        break;

                    default:
                        UserInterface.DisplayMessage("Improper parameters passed in.");
                        UserInterface.DisplayMessage("Update was unsuccessful. Please try your updates again");
                        break;
                }
                db.SubmitChanges();
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
            
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
            Animal animalFromDb = null;
            Client clientFromDb = null;
            try
            {
                animalFromDb = db.Animals.Where(c => c.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No animals have an AnimalId that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == client.ClientId).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No clients have a client ID that matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }

            Adoption animalAdoption = new Adoption();
            animalAdoption.AnimalId = animalFromDb.AnimalId;
            animalAdoption.ClientId = clientFromDb.ClientId;
            animalAdoption.ApprovalStatus = "Pending";
            animalAdoption.AdoptionFee = 100;
            animalAdoption.PaymentCollected = true;    

            db.Adoptions.InsertOnSubmit(animalAdoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()  
        {
            var animalsPendingAdoption = db.Adoptions.Where(p => p.ApprovalStatus == "Pending");
            
            return animalsPendingAdoption;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            //Search DB for this particular adoption
            Adoption adoptionFromDb = null;
            try
            {
                adoptionFromDb = db.Adoptions.Where(c => c.AnimalId == adoption.AnimalId).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No adoption matches the ID passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            Animal animalFromDb = db.Animals.Where(c => c.AnimalId == adoption.AnimalId).Single();
            if (isAdopted)
            {
                animalFromDb.AdoptionStatus = "Adopted";
                adoptionFromDb.ApprovalStatus = "Approved";
            }
            else
            {
                animalFromDb.AdoptionStatus = "Not Adopted";
                adoptionFromDb.ApprovalStatus = "Not Approved";
            }
            db.SubmitChanges();
        }

        internal static void RemoveAdoption(int animalId, int clientId)         //FOLLOW UP WITH DAVID
        {           
            Adoption adoptionFromDb = null;
            try
            {
                adoptionFromDb = db.Adoptions.Where(c => c.AnimalId == animalId && c.ClientId == clientId).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No record of adoption matches the values passed in.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
           
            adoptionFromDb.PaymentCollected = false;
            adoptionFromDb.ApprovalStatus = "Not approved";

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
            catch (InvalidOperationException)
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
            Shot thisShot = null;
            Animal animalWithShot = null;
            try
            {
                thisShot = db.Shots.Where(c => c.Name == shotName).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No record of this particular shot can be found.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            try
            {
                animalWithShot = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();
            }
            catch (InvalidOperationException)
            {
                UserInterface.DisplayMessage("No record of this animal can be found.");
                UserInterface.DisplayMessage("No updates have been made.");
                return;
            }
            AnimalShot shotToAdd = new AnimalShot();
            shotToAdd.AnimalId = animalWithShot.AnimalId;
            shotToAdd.ShotId = thisShot.ShotId;
            shotToAdd.DateReceived = DateTime.Today;

            db.AnimalShots.InsertOnSubmit(shotToAdd);
            db.SubmitChanges();

        }
    }
}