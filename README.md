
	NOT : L�tfen projeyi ilk a�t���n�zda Package Manager �zerinden "dotnet restore" komutunu �al��t�r�n

Bu projede, Generic Repository Design Patternde olan bir sorunsal� ve Service Oriented Architecture yakla��m�, best practice yaz�m� g�stermek ama�lanm��t�r.

Bilindi�i �zere Generic Repository, kendisine verilen herhangi bir DatabaseContext class'i i�erisinde bulunan t�m DbSet'ler yani Database i�indeki t�m tablolar i�in bir sefer yaz�lm�� olan ve  "BASE" olarak tan�mlanm�� - belirlenmi� olan CRUD (Create - Read - Update - Delete) fonksiyonlar�na ula�maya imk�n sa�layan, bunlar� virtual olarak tan�mlanm��sa ihtiya�lar do�rultusunda Database i�inde ki herhangi bir tablo i�in override ederek �zelle�tirebilece�imiz bir �r�nt� - yakla��m bi�imidir.

	Generic Repository Design Pattern'de ki sorun :
		Generic Repository'in temelinde yatan her tablo Base olarak belirlenmi� olan t�m fonksiyonlara ula�abilir durumunu bar�nd�rmaktad�r fakat, baz� tablolar 
sadece Insert fonksiyonlar�na sahip olmas� gerekirken, Base olarak belirlenmi� olan class'tan miras ald��� i�in ihtiyac� olmasa bile Update ve Delete 
fonksiyonlar�na da otomatik olarak sahip olmu� olacakt�r. Yani;

	NOT : Kodlar .Net Core EntityFrameworkCore'a g�re yaz�lm��t�r.
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

	G�r�ld��� �zere yukar�da IRepositoryBase<T> generic interfacesi ve RepositoryBase<T> class'� generic li�i ifade etmek isteyen T type i�in gelecek 
t�m Context s�n�f� (DatabaseContext) i�inde ki tablolara denk gelen classlar i�in burada yaz�lm�� olan t�m fonksiyonlara eri�me hakk�na sahip olacaklard�r.
Bu �rnek i�in DatabaseContext s�n�f�m�z;

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

	�eklindedir. G�r�ld��� �zere DatabaseContext s�n�f�m�z i�erisinde User ve Product ad�nda olan iki tane tablomuz var. IRepositoryBase<T> i�erisinde
signature olarak tan�mlanm�� olan ve RepositoryBase<T> class' i�erisinde implementation lar� yap�lan bu fonksiyonlar, RepositoryBase<T> class'� ve 
IRepositoryBase<T> interfacesi'nden miras alacak olan tablolar�n kendilerine ait Repository class'lar�n�n �zerinden veya direkt olarak RepositoryBase<T>'den 
instance �reterek <T> yerine gelen classlar i�in, o class'�n bu t�m fonksiyonlara ihtiyac� olup olmamas�na bak�lmadan direkt olarak kullanma imk�n�na sahip olacakt�r. Yani;

	.Net Core Console projesi i�erisinde olan;

		Instance y�ntemi ->

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

		G�r�lece�i �zere, RepositoryBase<T> den DatabaseContext i�erisinde tan�mlanm�� olan DbSet<TableName> e g�re yani Database'imiz i�inde olan 
Tablolar�m�za �zel bir instance �retti�imizde, o tablonun Insert, Update veya Delete fonksiyonlar�na ihtiya� olup olmamas�na bak�lmaks�z�n direkt 
olarak intellisense de IRepositoryBase'de tan�mlanm�� olan ve RepositoryBase'e implementation edilmi� olan t�m fonksiyonlar g�r�lmektedir ve 
kullan�ma a��k durumdad�r.

		Miras alan Repository class'lar� y�ntemi ->

		public interface IRepositoryUser : IRepositoryBase<User>
		{
			//Function signatures
		}

		public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
		{
			//NOT : Eger User tablosuna ait �zel bir fonksiyon yazmak istiyorsak bu fonksiyonlar� IRepositoryBase'e ve RepositoryBase de�il
			//IRepositoryUser da signature tan�mlamas� yap�p (Ayn� IRepositoryBase de oldu�u gibi) burada implementation yapmal�y�z. B�ylece 
			//yaz�lan fonksiyonlar Product i�in kullan�lamaz olmu� olacakt�r. Tablolar�n kendilerine �zel Repository classlarinin yaz�lmas�n�n as�l amac� budur.

			//Function implementations
		}


		public interface IRepositoryProduct : IRepositoryBase<Product>
		{
			//Function signatures
		}

		public class RepositoryProduct : RepositoryBase<Product> , IRepositoryProduct
		{
			//NOT : Eger Product tablosuna ait �zel bir fonksiyon yazmak istiyorsak bu fonksiyonlar� IRepositoryBase'e ve RepositoryBase de�il
			//IRepositoryProduct da signature tan�mlamas� yap�p (Ayn� IRepositoryBase de oldu�u gibi) burada implementation yapmal�y�z. B�ylece 
			//yaz�lan fonksiyonlar User i�in kullan�lamaz olmu� olacakt�r. Tablolar�n kendilerine �zel Repository classlarinin yaz�lmas�n�n as�l amac� budur.

			//Function implementations
		}

		.Net Core Console projesi i�erisinde olan;

			�zel Repository class'� y�ntemi ->

		class Program
		{
			#region �zel Repository Instance'leri
			
			IRepositoryUser _UserRepository = new RepositoryUser();
			IRepositoryProduct _ProductRepository = new RepositoryProduct();

			#endregion �zel Repository Instance'leri

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

		Yine g�r�lece�i �zere _UserRepository veya _ProductRepository �zerinden bir fonksiyona ula�mak istedi�imizde intellisense de
IRepositoryBase i�erisinde tan�mlanm�� olan ve RepositoryBase i�erisinde implementation edilmi� olan t�m fonksiyonlara eksiksiz 
ula�m�� oluyoruz. 
	
		NOT : Dikkat edilecek olursa e�er, RepositoryUser veya RepositoryProduct interface'leri IRepositoryBase'den miras almasa bile
RepositoryUser ve RepositoryProduct class'lar� RepositoryBase'den miras ald�klar� i�in IRepositoryBase i�inde ki fonksiyonlara ula�abilme
�zg�rl���ne sahip olacakt�r. Bu durumda Service Oriented Architecture - Interface Segeration mant���na uygun olmayan bir durum olu�acakt�r. Interface'lerin
olu�turulmas� ve fonksiyonlar�n signature tan�mlamalar�n�n interfacelerde yap�lmas�n�n amac�, class'lar kendi ba�lar�na hi�bir i�e yaramaz ve 
interface'ye ba��ml�d�r. Interfaceler �zerinde yetenek olarak tan�mlanmam�� olan hi�bir i�i yapamaz ve d��ar�ya a�amaz (Class'larin i�inde
�zel olarak yaz�lm�� olan private fonksiyonlar bunlara dahil de�ildir!) anlam�na gelmektedir. 
	
		�imdi bizim yapacak oldu�umuz t�m geli�tirmeler ve yaz�mlar�n hepsi, Service Oriented Architecture ve ihtiyac�n olan� kullan yani
Interface Segeration mant���na uygun hale getirmek i�indir. Hi�bir tablo, RepositoryBase i�erisinde yaz�lm�� olan ve ihtiyac� olmayan
fonksiyona ula�amaz mant��� ile �al��may� sa�lamakt�r.


		Bu sorunlar� ��zmek i�in en h�zl� iki t�rl� ��z�m olarak yakla��m sergileyebiliriz. Daha fazla ��z�m� belki de bu ��zd�mden daha
iyi bir ��z�m� siz uygulayabilirsiniz. Bu durumda l�tfen projemi fork layarak kendi geli�tirmenizi yap�p pull request talebi a��n�z :-)

		�lk ��z�m ->
		------------
			Tablolar�n ihtiya�lar� olabilecek oldu�u t�m CRUD fonksiyonlar�n�n kombinasyonu olan 4! tane class olu�turmak. Yani;

		RepositorySelectInsertUpdateDelete<T>, 
		RepositorySelectInsertUpdate<T>, RepositorySelectUpdateDelete<T>,
		RepositorySelectAndInsert<T>, RepositorySelectAndUpdate<T>, RepositorySelectAndDelete<T>, 
		RepositoryUpdateAndInsert<T>, RepositoryUpdateAndDelete<T>, RepositoryInsertAndDelete<T>, ....

	�eklinde g�r�ld��� �zere bu durum uzay�p gidiyor. Bu durumda sizin de tahmin edece�iniz gibi bu class'lar�n ve Interface'lerin
y�netimi a��r� zor ve kalabal�k olacakt�r. Bu ��z�m bence mant�ks�z bir ��z�md�r.

		�kinci ��z�m ->
		---------------

			Tablolar�n ihtiya�lar� olabilecek olan t�m CRUD fonksiyonlar�n�n par�alanm�� halde ki interfacelerinin yaz�lmas�. Yani;

		IRepositorySelect<T>, IRepositoryInsert<T>, IRepositoryUpdate<T> ve IRepositoryDelete<T> �eklinde. ��leri �u �ekil olmal�d�r:

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


		C - R - U - D fonksiyonlar�m�z i�in temel ihtiya�lara hizmet verebilecek t�m fonksiyonlar� i�eren interfaceleri haz�rlad�k.
�imdi bu interfacelerin bir yerde implementation edilmesi gerekiyor. Burada dikkat edilmesi gereken �ey .Net C# bir class, 
bir tane abstact veya class tan ve N (N = 999) tane interface'den miras alabilir. Bu y�zden biz bu 4 tane interface i�in 4 farkl�
class ta implementation yapmak yerine base yakla��m olan tek bir class �zerinde implementation i�lemini yapaca��z fakat tek bir 
farkl�l�kla bu implementationlar� ger�ekle�tirece�iz. Yani;

	NOT : Yukar�da tan�mlanm�� olan DatabaseContext ile devam edilecektir. User ve Product tablosu �zerinden �rnekler g�sterilecektir.
		
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

		Dikkat edilece�i �zere normalde public olan IRepositoryXXX<T> interfacelerimiz i�in implementation yapt���m�zda normalde t�m fonksiyonlar�n 
access modifierleri public olmas� gerekirken burada Interface ad� ile implementation yap�ld��� i�in herhangi bir access modifier veremiyoruz. Fakat
bu fonksiyonlar�n hepsi public tir. Dikkat edilmesi gereken tek �ey direkt olarak bu class'a ula�mak istersek bu fonksiyonlara private mi� gibi 
ula�amay�z. ��te bu y�ntem ile Service Oriented Architecture yakla��m�n� tam anlam�yla uygulam�� oluyourz.
	
		�imdi Product ve User classlar�m�z i�in tekrar birer tane IRepositoryXXX interfacesi ve RepositoryXXX class� olu�tural�m.

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

	G�r�ld��� �zere, normal - temel Generic Repository Design Pattern de oldu�u gibi tablolara �zel olan IRepositoryXXX ve RepositoryXXX classlarinda 
herhangi bir de�i�ikli�e gitmiyoruz. Dikkat ederseniz e�er normal - temel yakla��mdan tek farkl� olarak girdi�imiz k�s�m sadece ve sadece
RepositoryBase<T> i�inde yazd���m�z fonksiyonlar�n signature'lerine m�dahale ettik. Haricinde �imdiye kadar de�i�tirdi�imiz hi�bir �ey 
olmad�. �imdi bunlar� b�yle yapt�k fakat nas�l kullanaca��z veya �imdiye kadar yapm�� oldu�umuz kullan�mlardan farkl� bize ne katacak derseniz;

		.Net Core Console projesi i�erisinde olan;

		class Program
		{
			#region �zel Repository Instance'leri
			
			IRepositoryUser _UserRepository = new RepositoryUser();
			IRepositoryProduct _ProductRepository = new RepositoryProduct();

			#endregion �zel Repository Instance'leri

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

		G�r�lece�i �zere, RepositoryBase<T> i�erisinde IRepositoryDeletable<T> interfacesinden gelen DeleteItem fonksiyonu
implementation edildi�i halde ve bizim RepositoryUser class'�m�z RepositoryBase<T>'den (T:User) miras ald��� halde,
RepositoryBase<T> i�erisinde olan DeleteItem fonksiyonuna _UserRepository instancesinden ula�amad�k fakat _ProductRepository
instancesi �zerinden t�m fonksiyonlar�m�za ula�abilmi� olduk. 
		Bunun nedeni nedir? dersek ->

			Dikkat edilecek olursa 
				IRepositoryUser interfacesi IRepositorySelectable, IRepositoryInsertable ve IRepositoryUpdatable interfacelerinden
	inherit edilmekteyken;
				IRepositoryProduct interfacesi IRepositorySelectable, IRepositoryInsertable, IRepositoryUpdatable ve IRepositoryDeletable
	interfacelerinden inherit edilmektedir. 

	Bu da bize, IRepositoryUser �zerinden concrete edilmi� olan RepositoryUser class'�ndan bir instance �retilince IRepositoryDeletable
interfacesinde olan fonksiyonlara sahip olsa bile ula�amama - eri�ememe imk�n� sa�lam�� olmaktad�r. Yani Encapsulation yapt�k diyebiliriz.

		NOT : RepositoryUser veya RepositoryProduct classlar�ndan bir instance �retirken e�er bu instancelerin tiplerini direkt kendilerine
e�itlersek RepositoryBase �zerinden gelen hi�bir fonksiyonu g�rme imk�n�m�z olmayacakt�r. Service Oriented yakla��m� sayesinde gerekli
Encapsulation i�lemini bu �ekilde sa�lam�� oluyoruz. Yani class'�n hizmet verebilece�i tek �ey, inherit edilmi� oldu�u IRepositoryUser
veya IRepositoryProduct �zerinden gelecek olan fonksiyonlard�r.

            // RepositoryUser _RepositoryOfUser = new RepositoryUser(); -> Bu instance i�in hi�bir fonksiyona ula�amay�z. Sadece
			// RepositoryUser class'� object oldu�u i�in object �zerinden gelen temel fonksiyonlar gelir (GetType, Equals, ToString ve GetHashCode)

		T�m Repository ile alakal� olan instanceler, tablolar�n IRepositoryXXX �eklinde isme sahip interfaceler �zerinden yap�lmal�d�r.
			

	Art�k tam anlam�yla bir Service Oriented Architecture ve par�alanm�� bir Generic Repository Design Pattern'in �ok g�zel
bir haline ula�m�� olduk. 
		