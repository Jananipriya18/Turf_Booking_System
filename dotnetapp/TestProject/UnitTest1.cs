// using System.Collections.Generic;
// using System.Linq;
// using RideShare.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using NUnit.Framework;
// using System.Reflection;
// using Microsoft.AspNetCore.Mvc.ModelBinding;

// namespace RideShare.Tests
// {
//     [TestFixture]
//     public class RideSharingTests
//     {
//         private DbContextOptions<RideSharingDbContext> _dbContextOptions;
//         private RideSharingDbContext _context;

//         [SetUp]
//         public void Setup()
//         {
//             _dbContextOptions = new DbContextOptionsBuilder<RideSharingDbContext>()
//                 .UseInMemoryDatabase(databaseName: "TestDatabase")
//                 .Options;
//             _context = new RideSharingDbContext(_dbContextOptions);

//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         //{ "RideID", 1 },
//                         { "DepartureLocation", "Location A" },
//                         { "Destination", "Location B" },
//                         { "DateOfDeparture", DateTime.Parse("2023-08-30") },
//                         { "MaximumCapacity", 4 }
//                     };
//                 // Add test data to the in-memory database
//                 var ride = new Ride();
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Ride).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(ride, kvp.Value);
//                     }
//                 }

//                 dbContext.Rides.Add(ride);
//                 dbContext.SaveChanges();
//             }
//         }

//         [TearDown]
//         public void TearDown()
//         {
//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 // Clear the in-memory database after each test
//                 dbContext.Database.EnsureDeleted();
//             }
//         }

//         // test to check that JoinRide method in SlotController with successfull join redirects to Details method in RideController
//         [Test]
//         public void JoinRide_SlotController_ValidCommuter_JoinsSuccessfully_Redirect_to_Details_RideController()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string modelType = "RideShare.Models.Commuter";
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             Type controllerType2 = assembly.GetType(modelType);
//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 // Arrange
//                 var teamData = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe" },
//                         { "Email", "johndoe@example.com" },
//                         { "Phone", "1234567890" }
//                     };
//                 var commuter = new Commuter();
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter, kvp.Value);
//                     }
//                 }
//                 MethodInfo method = controllerType.GetMethod("JoinRide", new[] { typeof(int), controllerType2 });

//                 if (method != null)
//                 {
//                     var controller = Activator.CreateInstance(controllerType, _context);
//                     var result = method.Invoke(controller, new object[] { 1, commuter }) as RedirectToActionResult;


//                     //var result = slotController.JoinRide(1, commuter) as RedirectToActionResult;

//                     Assert.IsNotNull(result);

//                     Assert.AreEqual("Details", result.ActionName);
//                     Assert.AreEqual("Ride", result.ControllerName);
//                 }
//                 else
//                 {
//                     Assert.Fail();
//                 }
//             }
//         }

//         // test to check that JoinRide method in SlotController with successfull join adds commuter to the ride
//         [Test]
//         public void JoinRide_SlotController_ValidCommuter_Adds_Commuter_To_Ride_Successfully()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string modelType = "RideShare.Models.Commuter";
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             Type controllerType2 = assembly.GetType(modelType);
//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe" },
//                         { "Email", "johndoe@example.com" },
//                         { "Phone", "1234567890" }
//                     };
//                 var commuter = new Commuter();
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter, kvp.Value);
//                     }
//                 }
//                 MethodInfo method = controllerType.GetMethod("JoinRide", new[] { typeof(int), controllerType2 });

//                 if (method != null)
//                 {
//                     var ride1 = _context.Rides.Include(r => r.Commuters).ToList().FirstOrDefault(o => (int)o.GetType().GetProperty("RideID").GetValue(o) == 1);
//                     Assert.AreEqual(0, ride1.Commuters.Count);
//                     var controller = Activator.CreateInstance(controllerType, _context);
//                     var result = method.Invoke(controller, new object[] { 1, commuter }) as RedirectToActionResult;
//                     var ride = _context.Rides.Include(r => r.Commuters).ToList().FirstOrDefault(o => (int)o.GetType().GetProperty("RideID").GetValue(o) == 1);
//                     Assert.IsNotNull(ride);
//                     Assert.AreEqual(1, ride.Commuters.Count);

//                 }
//                 else
//                 {
//                     Assert.Fail();
//                 }
//             }
//         }




//         [Test]
//         public void JoinRide_SlotController_InvalidCommuter_Name_Email_Phone_are_required_ModelStateInvalid()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             string modelType = "RideShare.Models.Commuter";
//             Type modelType2 = assembly.GetType(modelType);

//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 // Arrange
//                 var slotController = Activator.CreateInstance(controllerType, dbContext);
//                 var commuter = Activator.CreateInstance(modelType2); // Invalid commuter with missing required fields

//                 // Add errors to ModelState
//                 var modelStateProperty = controllerType.GetProperty("ModelState");
//                 var modelState = modelStateProperty.GetValue(slotController) as ModelStateDictionary;
//                 modelState.AddModelError("Name", "Name is required");
//                 modelState.AddModelError("Email", "Email is required");
//                 modelState.AddModelError("Phone", "Phone is required");


//                 // Invoke JoinRide method using reflection
//                 MethodInfo joinRideMethod = controllerType.GetMethod("JoinRide", new[] { typeof(int), modelType2 });
//                 var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as ViewResult;

//                 // Assert
//                 Assert.IsNotNull(result);
//                 Assert.IsFalse(result.ViewData.ModelState.IsValid);
//                 Assert.AreEqual(3, result.ViewData.ModelState.ErrorCount);
//                 Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Name"));
//                 Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Email"));
//                 Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Phone"));
//             }
//         }


//         // test to check that JoinRide method in SlotController with invalid ride id returns NotFoundResult
//         [Test]
//         public void JoinRide_SlotController_RideNotFound_ReturnsNotFoundResult()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string modelType = "RideShare.Models.Commuter";
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             Type controllerType2 = assembly.GetType(modelType);
//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe" },
//                         { "Email", "johndoe@example.com" },
//                         { "Phone", "1234567890" }
//                     };
//                 var commuter = new Commuter();
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter, kvp.Value);
//                     }
//                 }
//                 MethodInfo method = controllerType.GetMethod("JoinRide", new[] { typeof(int) });

//                 if (method != null)
//                 {

//                     var controller = Activator.CreateInstance(controllerType, _context);
//                     var result = method.Invoke(controller, new object[] { 2 }) as NotFoundResult;
//                     Assert.IsNotNull(result);
//                 }
//                 else
//                 {
//                     Assert.Fail();
//                 }
//             }
//         }

//         // test to check that JoinRide method in SlotController throws exception when maximum capacity is reached
//         [Test]
//         public void JoinRide_SlotController_MaximumCapacityReached_ThrowsException()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string modelType = "RideShare.Models.Commuter";
//             string exception = "RideShare.Exceptions.RideSharingException";
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             Type controllerType2 = assembly.GetType(modelType);
//             Type exceptionType = assembly.GetType(exception);

//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe" },
//                         { "Email", "johndoe@example.com" },
//                         { "Phone", "1234567890" }
//                     };
//                 var teamData1 = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe1" },
//                         { "Email", "johndoe1@example.com" },
//                         { "Phone", "1234567891" }
//                     };
//                 var commuter = new Commuter();
//                 var commuter1 = new Commuter();
//                 foreach (var kvp in teamData1)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter1, kvp.Value);
//                     }
//                 }
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter, kvp.Value);
//                     }
//                 }
//                 MethodInfo method = controllerType.GetMethod("JoinRide", new[] { typeof(int) });


//                 var ride = _context.Rides.Include(r => r.Commuters).ToList().FirstOrDefault(o => (int)o.GetType().GetProperty("RideID").GetValue(o) == 1);
//                 ride.Commuters.Add(commuter1);
//                 ride.Commuters.Add(commuter);
//                 var propertyInfo1 = ride.GetType().GetProperty("MaximumCapacity");
//                 if (propertyInfo1 != null)
//                 {
//                     propertyInfo1.SetValue(ride, 2);
//                 }

//                 dbContext.SaveChanges();

//                 var teamData2 = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe2" },
//                         { "Email", "johndoe2@example.com" },
//                         { "Phone", "1234567892" }
//                     };
//                 var commuter2 = new Commuter();
//                 foreach (var kvp in teamData2)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter2, kvp.Value);
//                     }
//                 }
//                 if (method != null)
//                 {
//                     var controller = Activator.CreateInstance(controllerType, _context);
//                     var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(controller, new object[] { 1 }));

//                     var innerException = ex.InnerException;

//                     Assert.IsNotNull(innerException);
//                     var rideSharingExceptionType = exceptionType;
//                     bool isRideSharingException = rideSharingExceptionType.IsInstanceOfType(innerException);

//                     Assert.IsTrue(isRideSharingException, $"Expected inner exception of type {rideSharingExceptionType.FullName}");
//                 }
//             }
//         }

//         // test to check that JoinRide method in SlotController throws exception when maximum capacity is reached with correct message "Maximum capacity reached"
//         [Test]
//         public void JoinRide_SlotController_MaximumCapacityReached_ThrowsException_with_Message()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string modelType = "RideShare.Models.Commuter";
//             string exception = "RideShare.Exceptions.RideSharingException";
//             string controllerTypeName = "RideShare.Controllers.SlotController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             Type controllerType2 = assembly.GetType(modelType);
//             Type exceptionType = assembly.GetType(exception);

//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe" },
//                         { "Email", "johndoe@example.com" },
//                         { "Phone", "1234567890" }
//                     };
//                 var teamData1 = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe1" },
//                         { "Email", "johndoe1@example.com" },
//                         { "Phone", "1234567891" }
//                     };
//                 var commuter = new Commuter();
//                 var commuter1 = new Commuter();
//                 foreach (var kvp in teamData1)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter1, kvp.Value);
//                     }
//                 }
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter, kvp.Value);
//                     }
//                 }
//                 MethodInfo method = controllerType.GetMethod("JoinRide", new[] { typeof(int) });


//                 var ride = _context.Rides.Include(r => r.Commuters).ToList().FirstOrDefault(o => (int)o.GetType().GetProperty("RideID").GetValue(o) == 1);
//                 ride.Commuters.Add(commuter1);
//                 ride.Commuters.Add(commuter);
//                 var propertyInfo1 = ride.GetType().GetProperty("MaximumCapacity");
//                 if (propertyInfo1 != null)
//                 {
//                     propertyInfo1.SetValue(ride, 2);
//                 }

//                 dbContext.SaveChanges();

//                 var teamData2 = new Dictionary<string, object>
//                     {
//                         { "Name", "John Doe2" },
//                         { "Email", "johndoe2@example.com" },
//                         { "Phone", "1234567892" }
//                     };
//                 var commuter2 = new Commuter();
//                 foreach (var kvp in teamData2)
//                 {
//                     var propertyInfo = typeof(Commuter).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(commuter2, kvp.Value);
//                     }
//                 }
//                 if (method != null)
//                 {
//                     var controller = Activator.CreateInstance(controllerType, _context);
//                     //var ex =Assert.Throws<RideSharingException>(() => method.Invoke(controller, new object[] { 1, commuter }));
//                     //Console.WriteLine(ex.Message);
//                     var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(controller, new object[] { 1 }));

//                     // Retrieve the original exception thrown by the JoinRide method
//                     var innerException = ex.InnerException;

//                     // Assert that the inner exception is of type RideSharingException
//                     Assert.IsNotNull(innerException);
//                     var rideSharingExceptionType = exceptionType;
//                     bool isRideSharingException = rideSharingExceptionType.IsInstanceOfType(innerException);

//                     if (isRideSharingException)
//                     {
//                         var messageProperty = rideSharingExceptionType.GetProperty("Message");
//                         if (messageProperty != null)
//                         {
//                             var messageValue = messageProperty.GetValue(innerException);
//                             Assert.AreEqual("Maximum capacity reached", messageValue);
//                         }
//                     }
//                 }
//             }
//         }

//         [Test]
//         public void RideController_Delete_Method_ValidId_DeletesRideSuccessfully_Redirects_AvailableRides()
//         {
//             string assemblyName = "RideShare";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string controllerTypeName = "RideShare.Controllers.RideController";
//             Type controllerType = assembly.GetType(controllerTypeName);
//             using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//             {
//                 var teamData = new Dictionary<string, object>
//                     {
//                         //{ "RideID", 1 },
//                         { "DepartureLocation", "Location B" },
//                         { "Destination", "Location D" },
//                         { "DateOfDeparture", DateTime.Parse("2023-08-22") },
//                         { "MaximumCapacity", 4 }
//                     };
//                 var ride = new Ride();
//                 foreach (var kvp in teamData)
//                 {
//                     var propertyInfo = typeof(Ride).GetProperty(kvp.Key);
//                     if (propertyInfo != null)
//                     {
//                         propertyInfo.SetValue(ride, kvp.Value);
//                     }
//                 }

//                 dbContext.Rides.Add(ride);
//                 dbContext.SaveChanges();

//                 // Arrange
//                 MethodInfo deleteMethod = controllerType.GetMethod("Delete", new[] { typeof(int) });
//                 if (deleteMethod != null)
//                 {
//                     var controller = Activator.CreateInstance(controllerType, dbContext);
//                     var ridesBeforeDelete = dbContext.Rides.ToList();
//                     Console.WriteLine("count" + ridesBeforeDelete.Count);
//                     var rideIdToDelete = ridesBeforeDelete.FirstOrDefault()?.RideID ?? -1; 

//                     // Act
//                     var result = deleteMethod.Invoke(controller, new object[] { rideIdToDelete }) as RedirectToActionResult;

//                     // Assert
//                     Assert.IsNotNull(result);
//                     Assert.AreEqual("AvailableRides", result.ActionName); 
//                     var ridesAfterDelete = dbContext.Rides.ToList();
//                     Console.WriteLine("count" + ridesAfterDelete.Count);

//                     Assert.AreEqual(ridesBeforeDelete.Count - 1, ridesAfterDelete.Count); // Check if the number of rides decreased by 1
//                     Assert.IsNull(ridesAfterDelete.FirstOrDefault(r => r.RideID == rideIdToDelete)); // Check if the deleted ride is not present
//                 }
//                 else
//                 {
//                     Assert.Fail("Delete method not found in RideController.");
//                 }
//             }
//         }


//         //     [Test]
//         // public void JoinRide_DestinationSameAsDeparture_ReturnsViewWithValidationError()
//         // {
//         //     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//         //     {
//         //         // Arrange
//         //         var slotController = new SlotController(dbContext);
//         //         var commuter = new Commuter
//         //         {
//         //             Name = "John Doe",
//         //             Email = "johndoe@example.com",
//         //             Phone = "1234567890"
//         //         };

//         //         // Act
//         //         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         //         ride.Destination = ride.DepartureLocation; // Set the destination as the same as departure
//         //         dbContext.SaveChanges();

//         //         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         //         // Assert
//         //         Assert.IsNotNull(result);
//         //         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         //         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Destination"));
//         //     }
//         // }

//         // [Test]
//         // public void JoinRide_MaximumCapacityNotPositiveInteger_ReturnsViewWithValidationError()
//         // {
//         //     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//         //     {
//         //         // Arrange
//         //         var slotController = new SlotController(dbContext);
//         //         var commuter = new Commuter
//         //         {
//         //             Name = "John Doe",
//         //             Email = "johndoe@example.com",
//         //             Phone = "1234567890"
//         //         };

//         //         // Act
//         //         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         //         ride.MaximumCapacity = -5; // Set a negative value for MaximumCapacity
//         //         dbContext.SaveChanges();

//         //         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         //         // Assert
//         //         Assert.IsNotNull(result);
//         //         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         //         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("MaximumCapacity"));
//         //     }
//         // }


//         // Test to check whether Ride Models Class exists
//         [Test]
//         public void Ride_Models_ClassExists()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             Assert.IsNotNull(RideType);
//         }

//         // Test to check whether Commuter Models Class exists
//         [Test]
//         public void Commuter_Models_ClassExists()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             Assert.IsNotNull(CommuterType);
//         }


//         // Test to check that RideSharingDbContext Contains DbSet for model Ride
//         [Test]
//         public void RideSharingDbContext_ContainsDbSet_Ride()
//         {
//             Assembly assembly = Assembly.GetAssembly(typeof(RideSharingDbContext));
//             Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
//             if (contextType == null)
//             {
//                 Assert.Fail("No DbContext found in the assembly");
//                 return;
//             }
//             Type RideType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Ride");
//             if (RideType == null)
//             {
//                 Assert.Fail("No DbSet found in the DbContext");
//                 return;
//             }
//             var propertyInfo = contextType.GetProperty("Rides");
//             if (propertyInfo == null)
//             {
//                 Assert.Fail("Rides property not found in the DbContext");
//                 return;
//             }
//             else
//             {
//                 Assert.AreEqual(typeof(DbSet<>).MakeGenericType(RideType), propertyInfo.PropertyType);
//             }
//         }

//         // Test to check that RideSharingDbContext Contains DbSet for model Commuter
//         [Test]
//         public void RideSharingDbContext_ContainsDbSet_Commuter()
//         {
//             Assembly assembly = Assembly.GetAssembly(typeof(RideSharingDbContext));
//             Type contextType = assembly.GetTypes().FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t));
//             if (contextType == null)
//             {
//                 Assert.Fail("No DbContext found in the assembly");
//                 return;
//             }
//             Type CommuterType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Commuter");
//             if (CommuterType == null)
//             {
//                 Assert.Fail("No DbSet found in the DbContext");
//                 return;
//             }
//             var propertyInfo = contextType.GetProperty("Commuters");
//             if (propertyInfo == null)
//             {
//                 Assert.Fail("Commuters property not found in the DbContext");
//                 return;
//             }
//             else
//             {
//                 Assert.AreEqual(typeof(DbSet<>).MakeGenericType(CommuterType), propertyInfo.PropertyType);
//             }
//         }

//         // Test to Check Commuter Models Property CommuterID Exists with correcct datatype int    
//         [Test]
//         public void Commuter_CommuterID_PropertyExists_ReturnExpectedDataTypes_int()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = CommuterType.GetProperty("CommuterID");
//             Assert.IsNotNull(propertyInfo, "Property CommuterID does not exist in Commuter class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(int), expectedType, "Property CommuterID in Commuter class is not of type int");
//         }

//         // Test to Check Commuter Models Property Name Exists with correcct datatype string    
//         [Test]
//         public void Commuter_Name_PropertyExists_ReturnExpectedDataTypes_string()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = CommuterType.GetProperty("Name");
//             Assert.IsNotNull(propertyInfo, "Property Name does not exist in Commuter class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(string), expectedType, "Property Name in Commuter class is not of type string");
//         }

//         // Test to Check Commuter Models Property Email Exists with correcct datatype string    
//         [Test]
//         public void Commuter_Email_PropertyExists_ReturnExpectedDataTypes_string()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = CommuterType.GetProperty("Email");
//             Assert.IsNotNull(propertyInfo, "Property Email does not exist in Commuter class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(string), expectedType, "Property Email in Commuter class is not of type string");
//         }

//         // Test to Check Commuter Models Property Phone Exists with correcct datatype string    
//         [Test]
//         public void Commuter_Phone_PropertyExists_ReturnExpectedDataTypes_string()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = CommuterType.GetProperty("Phone");
//             Assert.IsNotNull(propertyInfo, "Property Phone does not exist in Commuter class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(string), expectedType, "Property Phone in Commuter class is not of type string");
//         }

//         // Test to Check Commuter Models Property RideID Exists with correcct datatype int    
//         [Test]
//         public void Commuter_RideID_PropertyExists_ReturnExpectedDataTypes_int()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Commuter";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type CommuterType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = CommuterType.GetProperty("RideID");
//             Assert.IsNotNull(propertyInfo, "Property RideID does not exist in Commuter class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(int), expectedType, "Property RideID in Commuter class is not of type int");
//         }

//         // Test to Check Ride Models Property RideID Exists with correcct datatype int    
//         [Test]
//         public void Ride_RideID_PropertyExists_ReturnExpectedDataTypes_int()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = RideType.GetProperty("RideID");
//             Assert.IsNotNull(propertyInfo, "Property RideID does not exist in Ride class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(int), expectedType, "Property RideID in Ride class is not of type int");
//         }

//         // Test to Check Ride Models Property DepartureLocation Exists with correcct datatype string    
//         [Test]
//         public void Ride_DepartureLocation_PropertyExists_ReturnExpectedDataTypes_string()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = RideType.GetProperty("DepartureLocation");
//             Assert.IsNotNull(propertyInfo, "Property DepartureLocation does not exist in Ride class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(string), expectedType, "Property DepartureLocation in Ride class is not of type string");
//         }

//         // Test to Check Ride Models Property Destination Exists with correcct datatype string    
//         [Test]
//         public void Ride_Destination_PropertyExists_ReturnExpectedDataTypes_string()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = RideType.GetProperty("Destination");
//             Assert.IsNotNull(propertyInfo, "Property Destination does not exist in Ride class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(string), expectedType, "Property Destination in Ride class is not of type string");
//         }

//         // Test to Check Ride Models Property DateOfDeparture Exists with correcct datatype DateTime    
//         [Test]
//         public void Ride_DateOfDeparture_PropertyExists_ReturnExpectedDataTypes_DateTime()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = RideType.GetProperty("DateOfDeparture");
//             Assert.IsNotNull(propertyInfo, "Property DateOfDeparture does not exist in Ride class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(DateTime), expectedType, "Property DateOfDeparture in Ride class is not of type DateTime");
//         }

//         // Test to Check Ride Models Property MaximumCapacity Exists with correcct datatype int    
//         [Test]
//         public void Ride_MaximumCapacity_PropertyExists_ReturnExpectedDataTypes_int()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Models.Ride";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideType = assembly.GetType(typeName);
//             PropertyInfo propertyInfo = RideType.GetProperty("MaximumCapacity");
//             Assert.IsNotNull(propertyInfo, "Property MaximumCapacity does not exist in Ride class");
//             Type expectedType = propertyInfo.PropertyType;
//             Assert.AreEqual(typeof(int), expectedType, "Property MaximumCapacity in Ride class is not of type int");
//         }



//         //        [Test]
//         //public void test_case12()
//         //{
//         //    using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//         //    {
//         //        // Arrange
//         //        var slotController = new SlotController(dbContext);
//         //        var commuter = new Commuter
//         //        {
//         //            Name = "John Doe",
//         //            Email = "johndoe@example.com",
//         //            Phone = "1234567890"
//         //        };

//         //        // Act
//         //        var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         //        ride.Destination = ride.DepartureLocation; // Set the destination as the same as departure
//         //        dbContext.SaveChanges();

//         //        var result = slotController.JoinRide(1, commuter) as ViewResult;

//         //        // Assert
//         //        Assert.IsNotNull(result);
//         //        Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         //        Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Destination"));
//         //    }
//         //}

//         //[Test]
//         //public void test_case13()
//         //{
//         //    using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//         //    {
//         //        // Arrange
//         //        var slotController = new SlotController(dbContext);
//         //        var commuter = new Commuter
//         //        {
//         //            Name = "John Doe",
//         //            Email = "johndoe@example.com",
//         //            Phone = "1234567890"
//         //        };

//         //        // Act
//         //        var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         //        ride.MaximumCapacity = -5; // Set a negative value for MaximumCapacity
//         //        dbContext.SaveChanges();

//         //        var result = slotController.JoinRide(1, commuter) as ViewResult;

//         //        // Assert
//         //        Assert.IsNotNull(result);
//         //        Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         //        Assert.IsTrue(result.ViewData.ModelState.ContainsKey("MaximumCapacity"));
//         //    }
//         //}

//         // Test to Check RideController Controllers Method AvailableRides with no parameter Returns IActionResult
//         [Test]
//         public void RideController_AvailableRides_Method_with_NoParams_Returns_IActionResult()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Controllers.RideController";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideControllerType = assembly.GetType(typeName);
//             MethodInfo methodInfo = RideControllerType.GetMethod("AvailableRides", Type.EmptyTypes);
//             Assert.AreEqual(typeof(IActionResult), methodInfo.ReturnType, "Method AvailableRides in RideController class is not of type IActionResult");
//         }

//         // Test to Check RideController Controllers Method Details with parameter int Returns IActionResult
//         [Test]
//         public void RideController_Details_Method_Invokes_with_int_Param_Returns_IActionResult()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Controllers.RideController";
//             Assembly assembly = Assembly.Load(assemblyName);
//             Type RideControllerType = assembly.GetType(typeName);
//             object instance = Activator.CreateInstance(RideControllerType, _context);
//             MethodInfo methodInfo = RideControllerType.GetMethod("Details", new[] { typeof(int) });
//             object result = methodInfo.Invoke(instance, new object[] { default(int) });
//             Assert.IsNotNull(result, "Result should not be null");
//         }

//         // Test to Check RideController Controllers Method Create with parameter Ride Returns IActionResult
//         [Test]
//         public void RideController_Delete_Method_Invokes_with_RideID_Param_Returns_IActionResult()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Controllers.RideController";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string typeName1 = "RideShare.Models.Ride";
//             Type RideType = assembly.GetType(typeName1);
//             object instance1 = Activator.CreateInstance(RideType);
//             Type RideControllerType = assembly.GetType(typeName);
//             object instance = Activator.CreateInstance(RideControllerType, _context);
//             MethodInfo methodInfo = RideControllerType.GetMethod("Delete", new Type[] { typeof(int) });
//             object result = methodInfo.Invoke(instance, new object[] { 1 });
//             Assert.IsNotNull(result, "Result should not be null");
//             Assert.AreEqual(typeof(IActionResult), methodInfo.ReturnType, "Method Delete in RideController class is not of type IActionResult");
//         }

//         [Test]
//         public void RideController_DeleteConfirm_Method_Invokes_with_RideID_Param_Returns_IActionResult()
//         {
//             string assemblyName = "RideShare";
//             string typeName = "RideShare.Controllers.RideController";
//             Assembly assembly = Assembly.Load(assemblyName);
//             string typeName1 = "RideShare.Models.Ride";
//             Type RideType = assembly.GetType(typeName1);
//             object instance1 = Activator.CreateInstance(RideType);
//             Type RideControllerType = assembly.GetType(typeName);
//             object instance = Activator.CreateInstance(RideControllerType, _context);
//             MethodInfo methodInfo = RideControllerType.GetMethod("DeleteConfirm", new Type[] { typeof(int) });
//             object result = methodInfo.Invoke(instance, new object[] { 1 });
//             Assert.IsNotNull(result, "Result should not be null");
//             Assert.AreEqual(typeof(IActionResult), methodInfo.ReturnType, "Method DeleteConfirm in RideController class is not of type IActionResult");
//         }
//     }

// }