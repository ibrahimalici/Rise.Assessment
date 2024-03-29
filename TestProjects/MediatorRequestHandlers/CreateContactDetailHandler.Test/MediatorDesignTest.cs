using ContactsAPI.Application.ContactsInfo.Commands;
using ContactsAPI.Application.ContactSubDetails.Commands;
using ContactsAPI.Entities;
using ContactsAPI.Persistance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MediatorDesign.CRUD.Test
{
    public class MediatorDesignTest
    {
        private Mock<IConfiguration> _mock;
        private CreateContactDetailHandler createContactDetailHandler;
        private CreateContactHandler createContactHandler;
        private UpdateContactHandler updateContactHandler;
        private UpdateContactDetailHandler updateContactDetailHandler;
        private DeleteContactHandler deleteContactHandler;
        private DeleteContactDetailHandler deleteContactDetailHandler;
        private DatabaseContext db;
        private readonly SqliteConnection connection;
        public MediatorDesignTest()
        {
            _mock = new Mock<IConfiguration>();

            connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(connection).Options;
            db = new DatabaseContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        #region .EF Crud Test

        #region .AddNew
        [Fact]
        public async void ContactAndContactDetail_AddNew_IsSuccessfully()
        {
            bool result = await AddData();

            Assert.True(result);
        }
        #endregion

        #region .Update Datas
        [Fact]
        public async Task ContactAndContactDetail_UpdateDatas_IsSuccessfully()
        {
            await AddData();

            Contact contact = await db.Contacts.FirstOrDefaultAsync();
            contact.Company = "Deneme A.�.";
            await db.SaveChangesAsync();

            ContactDetail contactDetail = await db.ContactDetails.Where(o=>o.ContactId == contact.ContactId).FirstOrDefaultAsync();
            contactDetail.Description = "test edildi";
            int result = await db.SaveChangesAsync();

            Assert.IsType<int>(result);
        }
        #endregion

        #region .Delete Datas
        public async Task ContactAndContactDetail_DeleteDatas_IsSuccessfully()
        {
            await AddData();

            Contact contact = await db.Contacts.FirstOrDefaultAsync();
            ContactDetail contactDetail = await db.ContactDetails.Where(o => o.ContactId == contact.ContactId).FirstOrDefaultAsync();

            db.ContactDetails.Remove(contactDetail);
            db.Contacts.Remove(contact);
            int result = await db.SaveChangesAsync();

            Assert.IsType<int>(result);
        }
        #endregion

        #endregion

        #region .Mediator Test

        #region .CreateContactHandler
        [Theory]
        [InlineData("Metin", "aaa", "")]
        [InlineData("Ali", "bbb", "")]
        [InlineData("Feyyaz", "ccc", "")]
        public async void CreateContactHandler_HandlerAction_IsSuccessfully(string Name, string Surename, string Company)
        {
            createContactHandler = new CreateContactHandler(db);
            var result = await createContactHandler.Handle(new CreateContactCommand
            {
                Company = Company,
                Name = Name,
                Surename = Surename
            }, new System.Threading.CancellationToken());

            Assert.IsType<Guid>(result);
        }
        #endregion

        #region .CreateContactDetailHandler
        [Theory]
        [InlineData(1, "05371654987")]
        [InlineData(2, "asda@com.tr")]
        [InlineData(3, "�STANBUL")]
        public async void CreateContactDetailHandler_HandleAction_IsSuccessfully(int ContactDetailType, string Description)
        {
            await AddData();
            var firstContact = await db.Contacts.FirstOrDefaultAsync();
            Guid ContactId = firstContact.ContactId;

            createContactDetailHandler = new CreateContactDetailHandler(db);
            var result = await createContactDetailHandler.Handle(new CreateContactDetailCommand
            {
                ContactDetailType = (SharedLibrary.Domains.ContactDetailType)ContactDetailType,
                ContactId = ContactId,
                Description = Description
            }, new System.Threading.CancellationToken());

            Assert.IsType<Guid>(result);
        }
        #endregion

        #region .UpdateContactHandler
        public async void UpdateContactHandler_HandlerAction_IsSuccessfully()
        {
            await AddData();
            var firstContact = await db.Contacts.FirstOrDefaultAsync();

            updateContactHandler = new UpdateContactHandler(db);
            var result = await updateContactHandler.Handle(new UpdateContactCommand
            {
                ContactId = firstContact.ContactId,
                Company = "Deneme A.�.",
                Name = firstContact.Name,
                Surename = firstContact.Surename
            }, new System.Threading.CancellationToken());

            Assert.True(result);
        }
        #endregion

        #region .UpdateContactDetailHandler
        public async void UpdateContactDetailHandler_HandlerAction_IsSuccessfully()
        {
            await AddData();
            var firstContact = await db.ContactDetails.FirstOrDefaultAsync();

            updateContactDetailHandler = new UpdateContactDetailHandler(db);
            var result = await updateContactDetailHandler.Handle(new UpdateContactDetailCommand
            {
                ContactId= firstContact.ContactId,
                ContactDetailId = firstContact.ContactDetailId,
                ContactDetailType = SharedLibrary.Domains.ContactDetailType.Location,
                Description = "GAZ�ANTEP"
            }, new System.Threading.CancellationToken());

            Assert.True(result);
        }
        #endregion

        #region .DeleteContactHandler
        public async void DeleteContactHandler_HandlerAction_IsSuccessfully()
        {
            await AddData();
            var firstContact = await db.Contacts.FirstOrDefaultAsync();

            deleteContactHandler = new DeleteContactHandler(db);
            var result = await deleteContactHandler.Handle(new DeleteContactCommand
            {
                ContactId = firstContact.ContactId,
            }, new System.Threading.CancellationToken());

            Assert.True(result);
        }
        #endregion

        #region .DeleteContactDetailHandler
        public async void DeleteContactDetailHandler_HandlerAction_IsSuccessfully()
        {
            await AddData();
            var firstContact = await db.ContactDetails.FirstOrDefaultAsync();

            deleteContactDetailHandler = new DeleteContactDetailHandler(db);
            var result = await deleteContactDetailHandler.Handle(new DeleteContactDetailCommand
            {
                ContactDetailId = firstContact.ContactDetailId,
            }, new System.Threading.CancellationToken());

            Assert.True(result);
        }
        #endregion

        #endregion


        #region AddData
        public async Task<bool> AddData()
        {
            Guid g = Guid.Parse("efe6b3b6-18e3-4003-964a-48e5745aac3f");
            await db.Contacts.AddAsync(new ContactsAPI.Entities.Contact
            {
                ContactId = g,
                Name = "Hasan",
                Surename = "H�seyin",
                Company = "x"
            });
            Guid sg = Guid.Parse("9674d17a-4613-4465-ba3d-5a48c4835834");
            await db.ContactDetails.AddAsync(new ContactsAPI.Entities.ContactDetail
            {
                ContactId = g,
                ContactDetailId = sg,
                ContactDetailType = SharedLibrary.Domains.ContactDetailType.Email,
                Description = "deneme@deneme.com"
            });

            await db.SaveChangesAsync();

            return true;
        }
        #endregion
    }
}
