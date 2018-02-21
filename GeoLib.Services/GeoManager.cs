using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GeoLib.Contracts;
using GeoLib.Data.Entities;
using GeoLib.Data.Repositories;
using GeoLib.Data.Repository_Interfaces;

namespace GeoLib.Services
{
    //This attribute if true allows the client to show details about the exceptions
    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    // This is the real service, the implementation of the contract interface
    // this is the project that should be hosted for the clients to consume.
    public class GeoManager : IGeoService
    {
        private readonly IStateRepository _repState;
        private readonly IZipCodeRepository _repZip;

        public GeoManager()
        {
            
        }

        // This is simulating of what the dependence injection would do 
        // It was put in here temporarily for make it testable
        public GeoManager(IStateRepository repState, IZipCodeRepository repZip)
        {
            _repZip = repZip;
            _repState = repState;
        }
       
        public ZipCodeData GetZipInfo(string zip)
        {
            // verifying if is coming from unit test  or real call
            IZipCodeRepository rep = _repZip ?? new ZipCodeRepository();

            // Doing the encapsulated thing that it should 
            ZipCode zipCode = rep.GetByZip(zip);

            if (zipCode == null)
            {
                
                Exception ex = new InvalidOperationException("Zip code not found");

                //Every exception thrown by the service will be a FaultException 
                //but this will be considered as unhandled one and will receive a generic message
                //throw ex;

                //Even with IncludeExceptionDetailInFaults false is possible to send a fail message
                //this is the simplest form of handled FaultException just explaining the reason.
                //throw new FaultException(new FaultReason("Zip code not found"));

                //Is possible also to send the actual exception details like:
                //message, inner exception, stack trace, etc. 
                //just put the ex exception in a new exception detail.
                throw new FaultException<ExceptionDetail>(new ExceptionDetail(ex), "No records");

                // Custom error object.
                //NotFoundData data = new NotFoundData
                //{
                //    Message = "Zip code not found",
                //    When = DateTime.Now.ToString(),
                //    User = "Pedro"
                //};

                //It is possible even send a kind of exception like InvalidOperationException
                // or in this case a custom error object
                //throw new FaultException<NotFoundData>(data, "No records");
            }

            ZipCodeData result = new ZipCodeData
            {
                City = zipCode.City,
                State = zipCode.State.Abbreviation,
                ZipCode = zipCode.Zip
            };
            
            // returning what the client is expecting
            return result;
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            IStateRepository rep = _repState ?? new StateRepository();

            IEnumerable<State> states = rep.Get(primaryOnly);
            IEnumerable<string> result = states.Select(s => s.Abbreviation);

            return result;
        }

        public IEnumerable<ZipCodeData> GetZip(string state)
        {
            IZipCodeRepository rep = _repZip ?? new ZipCodeRepository();

            IEnumerable<ZipCode> zipCodes = rep.GetByState(state);
            IEnumerable<ZipCodeData> result = zipCodes.Select(s => new ZipCodeData
            {
                State = s.State.Abbreviation,
                ZipCode = s.Zip,
                City = s.City
            }).ToList();
            return result;
        }

        public IEnumerable<ZipCodeData> GetZip(string zip, int range)
        {
            IZipCodeRepository rep = _repZip ?? new ZipCodeRepository();

            ZipCode zipCode = rep.GetByZip(zip);
            IEnumerable<ZipCode> zipCodes = rep.GetZipsForRange(zipCode, range);
            IEnumerable<ZipCodeData> result = zipCodes.Select(s => new ZipCodeData
            {
                State = s.State.Abbreviation,
                ZipCode = s.Zip,
                City = s.City
            }).ToList();
            return result;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void UpdateZipCity(string zip, string city)
        {
            IZipCodeRepository rep = _repZip ?? new ZipCodeRepository();
            ZipCode zipCode = rep.GetByZip(zip);

            if (zipCode == null) return;

            zipCode.City = city;
            rep.Update(zipCode);
        }

        // The TransactionScopeRequired is what tells the service that this method is within a transaction
        // The TransactionAutoComplete allows the service to automatic close the transaction in case o f success.
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        public void UpdateZipCty(IEnumerable<ZipCityData> zipCityData)
        {
            IZipCodeRepository rep = _repZip ?? new ZipCodeRepository();
            //This below would be the efficient way to do 
            //var cityBatch = zipCityData.ToDictionary(c => c.ZipCode, c => c.City);
            //rep.UpdateZipBatch(cityBatch);

            
            //For simulate error in a transaction we gonna use this ugly code below
            int counter = 0;
            foreach (var cityData in zipCityData)
            {
                // in a operation without transaction only the first one will succeed.
                //counter++;
                //if (counter==2) throw new FaultException("Sorry, no can do !");

                ZipCode zip = rep.GetByZip(cityData.ZipCode);
                zip.City = cityData.City;
                ZipCode updatedZipCode = rep.Update(zip);
            }

            // To regain control of when and where the transaction is closed 
            // it needs to set the TransactionAutoCompleteto false, because the default is true
            // This code below closes the transaction
            OperationContext.Current.SetTransactionComplete();
        }
    }
}
