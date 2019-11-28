
	NOT : Lütfen projeyi ilk açtýðýnýzda Package Manager üzerinden "dotnet restore" komutunu çalýþtýrýn

Bu projede, Generic Repository Design Patternde olan bir sorunsalý ve Service Oriented Architecture yaklaþýmý, best practice yazýmý göstermek amaçlanmýþtýr.

Bilindiði üzere Generic Repository, kendisine verilen herhangi bir DatabaseContext class'i içerisinde bulunan tüm DbSet'ler yani Database içindeki tüm tablolar için bir sefer yazýlmýþ olan ve  "BASE" olarak tanýmlanmýþ - belirlenmiþ olan CRUD (Create - Read - Update - Delete) fonksiyonlarýna ulaþmaya imkân saðlayan, bunlarý virtual olarak tanýmlanmýþsa ihtiyaçlar doðrultusunda Database içinde ki herhangi bir tablo için override ederek özelleþtirebileceðimiz bir örüntü - yaklaþým biçimidir.

	Generic Repository Design Pattern'de ki sorun :
		Generic Repository'in temelinde yatan her tablo Base olarak belirlenmiþ olan tüm fonksiyonlara ulaþabilir durumunu barýndýrmaktadýr fakat, bazý tablolar 
sadece Insert fonksiyonlarýna sahip olmasý gerekirken, Base olarak belirlenmiþ olan class'tan miras aldýðý için ihtiyacý olmasa bile Update ve Delete 
fonksiyonlarýna da otomatik olarak sahip olmuþ olacaktýr. Yani;

	NOT : Kodlar .Net Core EntityFrameworkCore'a göre yazýlmýþtýr.
	NOT : Fonksiyonlar temsili olarak yazilmistir. Tam anlamiyla calismayabilir.


	public interface IRepositoryBase<T> where T : class
	{
		IQueryable<T> AllItem();
		IQueryable<T> AllItem(Expression<Func<T,bool>> predicate);
		T GetItem(object id);

		bool InsertItem(T item);

		bool DeleteItem(object id);

		T UpdateItem(object id, T item);
	}


	public class RepositoryBase<T> : IRepositoryBase<T> where T:class
	{
		private readonly DatabaseContext Context = new DatabaseContext();

		public virtual IQueryable<T> AllItem()
		{
			return this.Context.Set<T>()
							   .AsQueryable();
		}

		public virtual IQueryable<T> AllItem(Expression<Func<T,bool>> predicate)
		{
			return this.Context.Set<T>()
							   .Where(predicate: predicate)
							   .AsQueryable();

		}

		public virtual T GetItem(object id)
		{
			return this.Context.Find<T>(keyValues: id);
		}

		public virtual bool InsertItem(T item)
		{
			var insertedItem = this.Context.Set<T>().Add(entity: insertItem);
			return this.Context.SaveChanges() > 0 ? true : false;
		}

		public virtual bool DeleteItem(object id)
		{
			T getItem = this.GetItem(id : id);
			if(getItem != null)
			{
				var deleteItem = this.Context.Set<T>().Remove(entity: getItem);
				return this.Context.SaveChanges() > 0 ? true : false;
			}
			return false;
		}

		public virtual T UpdateItem(object id, T item)
		{
			T getItem = this.GetItem(id: id);
			if(getItem != null)
			{
				var updateItem = this.Context.Set<T>().Update(entity: updateItem);
				this.Context.SaveChanges();
				return updateItem.Entity;
			}
			return null;
		}
	}

	Görüldüðü üzere yukarýda IRepositoryBase<T> generic interfacesi ve RepositoryBase<T> class'ý generic liði ifade etmek isteyen T type için gelecek 
tüm Context sýnýfý (DatabaseContext) içinde ki tablolara denk gelen classlar için burada yazýlmýþ olan tüm fonksiyonlara eriþme hakkýna sahip olacaklardýr.
Bu örnek için DatabaseContext sýnýfýmýz;

	public class DatabaseContext : DbContext
	{
		public DatabaseContext(){ }

		public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

		public DbSet<Product> Products {get; set;}
		public DbSet<User> Users {get; set;}
	

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(local); Initial Catalog=TodoList; Integrated Security=true;");
            }
        }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>(entity => {
				
				//Gerekli entity tanimlamalari
				....
			});

			modelBuilder.Entity<User>(entity => {
				
				//Gerekli entity tanimlamalari
				....
			});
		}
	}

	Tablolar;

	public partial class User
    {
        public User()
        {
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

	public partial class Product
	{
		public int Id {get; set;}
		public string Name {get; set;}
		public int Quantity {get; set;}
		public decimal Price {get; set;}

		public DateTime CreationDate {get; set;}
		public Nullable<DateTime> UpdateDate {get; set;}
		public Nullable<DateTime> DeleteDate {get; set;}
	}

	þeklindedir. Görüldüðü üzere DatabaseContext sýnýfýmýz içerisinde User ve Product adýnda olan iki tane tablomuz var. IRepositoryBase<T> içerisinde
signature olarak tanýmlanmýþ olan ve RepositoryBase<T> class' içerisinde implementation larý yapýlan bu fonksiyonlar, RepositoryBase<T> class'ý ve 
IRepositoryBase<T> interfacesi'nden miras alacak olan tablolarýn kendilerine ait Repository class'larýnýn üzerinden veya direkt olarak RepositoryBase<T>'den 
instance üreterek <T> yerine gelen classlar için, o class'ýn bu tüm fonksiyonlara ihtiyacý olup olmamasýna bakýlmadan direkt olarak kullanma imkânýna sahip olacaktýr. Yani;

	.Net Core Console projesi içerisinde olan;

		Instance yöntemi ->

	class Program 
	{
		#region Instances
		IRepositoryBase<User> _UserRepository = new RepositoryBase<User>();
		
		IRepositoryBase<Product> _ProductRepository = new RepositoryBase<Product>();
		#endregion Instances
		
		static void Main(string[] args)
		{
			var userFunctions = _UserRepository.AllItem()
											   .AllItem(x=> x.Name == "First User")
											   .GetItem(id: 2)
											   .InsertItem(item : new User{ Name="New User Name", Surname="New User Surname" ... })
											   .DeleteItem(id: 2)
											   .UpdateItem(id: 2, item: new User{Id= 2, Name="Update User Name", Surname="Update User Surname" ...})
											   ;
			

			var productFunctions = _ProductRepository.AllItem()
													 .AllItem(x=> x.Name == "First Product")
													 .GetItem(id: 2)
													 .InsertItem(item : new Product{ Name="New Product Name", Quantity= 5, Price=1.4 ... })
													 .DeleteItem(id: 2)
													 .UpdateItem(id: 2, item: new Product{Id= 2, Name="New Product Name", Quantity= 5, Price=1.4 ...})
													 ;

		}
	}

		Görüleceði üzere, RepositoryBase<T> den DatabaseContext içerisinde tanýmlanmýþ olan DbSet<TableName> e göre yani Database'imiz içinde olan 
Tablolarýmýza özel bir instance ürettiðimizde, o tablonun Insert, Update veya Delete fonksiyonlarýna ihtiyaç olup olmamasýna bakýlmaksýzýn direkt 
olarak intellisense de IRepositoryBase'de tanýmlanmýþ olan ve RepositoryBase'e implementation edilmiþ olan tüm fonksiyonlar görülmektedir ve 
kullanýma açýk durumdadýr.

		Miras alan Repository class'larý yöntemi ->

		public interface IRepositoryUser : IRepositoryBase<User>
		{
			//Function signatures
		}

		public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
		{
			//NOT : Eger User tablosuna ait özel bir fonksiyon yazmak istiyorsak bu fonksiyonlarý IRepositoryBase'e ve RepositoryBase deðil
			//IRepositoryUser da signature tanýmlamasý yapýp (Ayný IRepositoryBase de olduðu gibi) burada implementation yapmalýyýz. Böylece 
			//yazýlan fonksiyonlar Product için kullanýlamaz olmuþ olacaktýr. Tablolarýn kendilerine özel Repository classlarinin yazýlmasýnýn asýl amacý budur.

			//Function implementations
		}


		public interface IRepositoryProduct : IRepositoryBase<Product>
		{
			//Function signatures
		}

		public class RepositoryProduct : RepositoryBase<Product> , IRepositoryProduct
		{
			//NOT : Eger Product tablosuna ait özel bir fonksiyon yazmak istiyorsak bu fonksiyonlarý IRepositoryBase'e ve RepositoryBase deðil
			//IRepositoryProduct da signature tanýmlamasý yapýp (Ayný IRepositoryBase de olduðu gibi) burada implementation yapmalýyýz. Böylece 
			//yazýlan fonksiyonlar User için kullanýlamaz olmuþ olacaktýr. Tablolarýn kendilerine özel Repository classlarinin yazýlmasýnýn asýl amacý budur.

			//Function implementations
		}

		.Net Core Console projesi içerisinde olan;

			Özel Repository class'ý yöntemi ->

		class Program
		{
			#region Özel Repository Instance'leri
			
			IRepositoryUser _UserRepository = new RepositoryUser();
			IRepositoryProduct _ProductRepository = new RepositoryProduct();

			#endregion Özel Repository Instance'leri

			static void Main(string[] args)
			{
				var userFunctions = _UserRepository.AllItem()
												   .AllItem(x=> x.Name == "First User")
												   .GetItem(id: 2)
												   .InsertItem(item : new User{ Name="New User Name", Surname="New User Surname" ... })
												   .DeleteItem(id: 2)
												   .UpdateItem(id: 2, item: new User{Id= 2, Name="Update User Name", Surname="Update User Surname" ...})
												   ;
			

				var productFunctions = _ProductRepository.AllItem()
														 .AllItem(x=> x.Name == "First Product")
														 .GetItem(id: 2)
														 .InsertItem(item : new Product{ Name="New Product Name", Quantity= 5, Price=1.4 ... })
														 .DeleteItem(id: 2)
														 .UpdateItem(id: 2, item: new Product{Id= 2, Name="New Product Name", Quantity= 5, Price=1.4 ...})
														 ;

			}
		}

		Yine görüleceði üzere _UserRepository veya _ProductRepository üzerinden bir fonksiyona ulaþmak istediðimizde intellisense de
IRepositoryBase içerisinde tanýmlanmýþ olan ve RepositoryBase içerisinde implementation edilmiþ olan tüm fonksiyonlara eksiksiz 
ulaþmýþ oluyoruz. 
	
		NOT : Dikkat edilecek olursa eðer, RepositoryUser veya RepositoryProduct interface'leri IRepositoryBase'den miras almasa bile
RepositoryUser ve RepositoryProduct class'larý RepositoryBase'den miras aldýklarý için IRepositoryBase içinde ki fonksiyonlara ulaþabilme
özgürlüðüne sahip olacaktýr. Bu durumda Service Oriented Architecture - Interface Segeration mantýðýna uygun olmayan bir durum oluþacaktýr. Interface'lerin
oluþturulmasý ve fonksiyonlarýn signature tanýmlamalarýnýn interfacelerde yapýlmasýnýn amacý, class'lar kendi baþlarýna hiçbir iþe yaramaz ve 
interface'ye baðýmlýdýr. Interfaceler üzerinde yetenek olarak tanýmlanmamýþ olan hiçbir iþi yapamaz ve dýþarýya açamaz (Class'larin içinde
özel olarak yazýlmýþ olan private fonksiyonlar bunlara dahil deðildir!) anlamýna gelmektedir. 
	
		Þimdi bizim yapacak olduðumuz tüm geliþtirmeler ve yazýmlarýn hepsi, Service Oriented Architecture ve ihtiyacýn olaný kullan yani
Interface Segeration mantýðýna uygun hale getirmek içindir. Hiçbir tablo, RepositoryBase içerisinde yazýlmýþ olan ve ihtiyacý olmayan
fonksiyona ulaþamaz mantýðý ile çalýþmayý saðlamaktýr.


		Bu sorunlarý çözmek için en hýzlý iki türlü çözüm olarak yaklaþým sergileyebiliriz. Daha fazla çözümü belki de bu çözdümden daha
iyi bir çözümü siz uygulayabilirsiniz. Bu durumda lütfen projemi fork layarak kendi geliþtirmenizi yapýp pull request talebi açýnýz :-)

		Ýlk çözüm ->
		------------
			Tablolarýn ihtiyaçlarý olabilecek olduðu tüm CRUD fonksiyonlarýnýn kombinasyonu olan 4! tane class oluþturmak. Yani;

		RepositorySelectInsertUpdateDelete<T>, 
		RepositorySelectInsertUpdate<T>, RepositorySelectUpdateDelete<T>,
		RepositorySelectAndInsert<T>, RepositorySelectAndUpdate<T>, RepositorySelectAndDelete<T>, 
		RepositoryUpdateAndInsert<T>, RepositoryUpdateAndDelete<T>, RepositoryInsertAndDelete<T>, ....

	þeklinde görüldüðü üzere bu durum uzayýp gidiyor. Bu durumda sizin de tahmin edeceðiniz gibi bu class'larýn ve Interface'lerin
yönetimi aþýrý zor ve kalabalýk olacaktýr. Bu çözüm bence mantýksýz bir çözümdür.

		Ýkinci çözüm ->
		---------------

			Tablolarýn ihtiyaçlarý olabilecek olan tüm CRUD fonksiyonlarýnýn parçalanmýþ halde ki interfacelerinin yazýlmasý. Yani;

		IRepositorySelect<T>, IRepositoryInsert<T>, IRepositoryUpdate<T> ve IRepositoryDelete<T> þeklinde. Ýçleri þu þekil olmalýdýr:

		public interface IRepositorySelectable<T> where T : class
		{
			IQueryable<T> AllItems();
			IQueryable<T> AllItems(Expression<Func<T,bool>> predicate);

			T GetItem(object id);
		}

		public interface IRepositoryInsertable<T> where T : class
		{
			bool InsertItem(T item);	
		}

		public interface IRepositoryUpdatable<T> where T : class
		{
			T UpdateItem(object id, T item);
		}

		public interface IRepositoryDeletable<T> where T : class
		{
			bool DeleteItem(object id);
		}


		C - R - U - D fonksiyonlarýmýz için temel ihtiyaçlara hizmet verebilecek tüm fonksiyonlarý içeren interfaceleri hazýrladýk.
Þimdi bu interfacelerin bir yerde implementation edilmesi gerekiyor. Burada dikkat edilmesi gereken þey .Net C# bir class, 
bir tane abstact veya class tan ve N (N = 999) tane interface'den miras alabilir. Bu yüzden biz bu 4 tane interface için 4 farklý
class ta implementation yapmak yerine base yaklaþým olan tek bir class üzerinde implementation iþlemini yapacaðýz fakat tek bir 
farklýlýkla bu implementationlarý gerçekleþtireceðiz. Yani;

	NOT : Yukarýda tanýmlanmýþ olan DatabaseContext ile devam edilecektir. User ve Product tablosu üzerinden örnekler gösterilecektir.
		
		public class RepositoryBase<T> : IRepositorySelectable<T>, IRepositoryInsertable<T>, IRepositoryUpdatable<T>, IRepositoryDeletable<T>
		where T : class 
		{
			private readonly DatabaseContext Context = new DatabaseContext();

			#region Select Functions
			IQueryable<T> IRepositorySelectable<T>.AllItems()
			{
			    return this.Context.Set<T>()
								   .AsQueryable();
			}

			IQueryable<T> IRepositorySelectable<T>.AllItems(Expression<Func<T, bool>> predicate)
			{
			    return this.Context.Set<T>()
								   .Where(predicate: predicate)
								   .AsQueryable();
			}

			T IRepositorySelectable<T>.GetItem(object id)
			{
			    return this.Context.Find<T>(keyValues: id);
			}
			#endregion Select Functions

			#region Insert Function
			bool IRepositoryInsertable<T>.InsertItem(T item)
			{
			    var insertedItem = this.Context.Set<T>().Add(entity: insertItem);
				return this.Context.SaveChanges() > 0 ? true : false;
			}
			#endregion Insert Function

			#region Update Function
			T IRepositoryUpdatable<T>.UpdateItem(object id, T item)
			{
			    T getItem = this.GetItem(id: id);
				if(getItem != null)
				{
					var updateItem = this.Context.Set<T>().Update(entity: updateItem);
					this.Context.SaveChanges();
					return updateItem.Entity;
				}
				return null;
			}
			#endregion Update Function

			#region Delete Function
			bool IRepositoryDeletable<T>.DeleteItem(object id)
			{
				T getItem = this.GetItem(id : id);
				if(getItem != null)
				{
					var deleteItem = this.Context.Set<T>().Remove(entity: getItem);
					return this.Context.SaveChanges() > 0 ? true : false;
				}
				return false;
			}
			#endregion Delete Function
		}

		Dikkat edileceði üzere normalde public olan IRepositoryXXX<T> interfacelerimiz için implementation yaptýðýmýzda normalde tüm fonksiyonlarýn 
access modifierleri public olmasý gerekirken burada Interface adý ile implementation yapýldýðý için herhangi bir access modifier veremiyoruz. Fakat
bu fonksiyonlarýn hepsi public tir. Dikkat edilmesi gereken tek þey direkt olarak bu class'a ulaþmak istersek bu fonksiyonlara private miþ gibi 
ulaþamayýz. Ýþte bu yöntem ile Service Oriented Architecture yaklaþýmýný tam anlamýyla uygulamýþ oluyourz.
	
		Þimdi Product ve User classlarýmýz için tekrar birer tane IRepositoryXXX interfacesi ve RepositoryXXX classý oluþturalým.

		public interface IRepositoryUser : IRepositorySelectable<User>, IRepositoryInsertable<User>, IRepositoryUpdatable<User>
		{
		}

		public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
		{
		}


		public interface IRepositoryProduct : IRepositorySelectable<Product>, IRepositoryInsertable<Product>, IRepositoryUpdatable<Product>, 
											  IRepositoryDeletable<Product>
		{
		}

		public class RepositoryProduct : RepositoryBase<Product>, IRepositoryProduct
		{
		}

	Görüldüðü üzere, normal - temel Generic Repository Design Pattern de olduðu gibi tablolara özel olan IRepositoryXXX ve RepositoryXXX classlarinda 
herhangi bir deðiþikliðe gitmiyoruz. Dikkat ederseniz eðer normal - temel yaklaþýmdan tek farklý olarak girdiðimiz kýsým sadece ve sadece
RepositoryBase<T> içinde yazdýðýmýz fonksiyonlarýn signature'lerine müdahale ettik. Haricinde þimdiye kadar deðiþtirdiðimiz hiçbir þey 
olmadý. Þimdi bunlarý böyle yaptýk fakat nasýl kullanacaðýz veya þimdiye kadar yapmýþ olduðumuz kullanýmlardan farklý bize ne katacak derseniz;

		.Net Core Console projesi içerisinde olan;

		class Program
		{
			#region Özel Repository Instance'leri
			
			IRepositoryUser _UserRepository = new RepositoryUser();
			IRepositoryProduct _ProductRepository = new RepositoryProduct();

			#endregion Özel Repository Instance'leri

			static void Main(string[] args)
			{
				var userFunctions = _UserRepository.AllItem()
												   .AllItem(x=> x.Name == "First User")
												   .GetItem(id: 2)
												   .InsertItem(item : new User{ Name="New User Name", Surname="New User Surname" ... })
												 //.DeleteItem(id: 2)
												   .UpdateItem(id: 2, item: new User{Id= 2, Name="Update User Name", Surname="Update User Surname" ...})
												   ;
			

				var productFunctions = _ProductRepository.AllItem()
														 .AllItem(x=> x.Name == "First Product")
														 .GetItem(id: 2)
														 .InsertItem(item : new Product{ Name="New Product Name", Quantity= 5, Price=1.4 ... })
														 .DeleteItem(id: 2)
														 .UpdateItem(id: 2, item: new Product{Id= 2, Name="New Product Name", Quantity= 5, Price=1.4 ...})
														 ;
			}
		}

		Görüleceði üzere, RepositoryBase<T> içerisinde IRepositoryDeletable<T> interfacesinden gelen DeleteItem fonksiyonu
implementation edildiði halde ve bizim RepositoryUser class'ýmýz RepositoryBase<T>'den (T:User) miras aldýðý halde,
RepositoryBase<T> içerisinde olan DeleteItem fonksiyonuna _UserRepository instancesinden ulaþamadýk fakat _ProductRepository
instancesi üzerinden tüm fonksiyonlarýmýza ulaþabilmiþ olduk. 
		Bunun nedeni nedir? dersek ->

			Dikkat edilecek olursa 
				IRepositoryUser interfacesi IRepositorySelectable, IRepositoryInsertable ve IRepositoryUpdatable interfacelerinden
	inherit edilmekteyken;
				IRepositoryProduct interfacesi IRepositorySelectable, IRepositoryInsertable, IRepositoryUpdatable ve IRepositoryDeletable
	interfacelerinden inherit edilmektedir. 

	Bu da bize, IRepositoryUser üzerinden concrete edilmiþ olan RepositoryUser class'ýndan bir instance üretilince IRepositoryDeletable
interfacesinde olan fonksiyonlara sahip olsa bile ulaþamama - eriþememe imkâný saðlamýþ olmaktadýr. Yani Encapsulation yaptýk diyebiliriz.

		NOT : RepositoryUser veya RepositoryProduct classlarýndan bir instance üretirken eðer bu instancelerin tiplerini direkt kendilerine
eþitlersek RepositoryBase üzerinden gelen hiçbir fonksiyonu görme imkânýmýz olmayacaktýr. Service Oriented yaklaþýmý sayesinde gerekli
Encapsulation iþlemini bu þekilde saðlamýþ oluyoruz. Yani class'ýn hizmet verebileceði tek þey, inherit edilmiþ olduðu IRepositoryUser
veya IRepositoryProduct üzerinden gelecek olan fonksiyonlardýr.

            // RepositoryUser _RepositoryOfUser = new RepositoryUser(); -> Bu instance için hiçbir fonksiyona ulaþamayýz. Sadece
			// RepositoryUser class'ý object olduðu için object üzerinden gelen temel fonksiyonlar gelir (GetType, Equals, ToString ve GetHashCode)

		Tüm Repository ile alakalý olan instanceler, tablolarýn IRepositoryXXX þeklinde isme sahip interfaceler üzerinden yapýlmalýdýr.
			

	Artýk tam anlamýyla bir Service Oriented Architecture ve parçalanmýþ bir Generic Repository Design Pattern'in çok güzel
bir haline ulaþmýþ olduk. 
		