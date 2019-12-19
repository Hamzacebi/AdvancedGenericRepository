
####	 NOT : Lütfen projeyi ilk açtığınızda Package Manager üzerinden "dotnet restore" komutunu çalıştırın

Bu projede, Generic Repository Design Patternde olan bir sorunsalı best practice yazımı göstermek amaçlanmıştır.

Bilindiği üzere Generic Repository, kendisine verilen herhangi bir DatabaseContext class'i içerisinde bulunan tüm DbSet'ler yani Database içindeki tüm tablolar için bir sefer yazılmış olan ve  "BASE" olarak tanımlanmış - belirlenmiş olan CRUD (Create - Read - Update - Delete) fonksiyonlarına ulaşmaya imkân sağlayan, bunları virtual olarak tanımlanmışsa ihtiyaçlar doğrultusunda Database içinde ki herhangi bir tablo için override ederek özelleştirebileceğimiz bir örüntü - yaklaşım biçimidir.

	Generic Repository Design Pattern'de ki sorun :
		
Generic Repository'in temelinde yatan her tablo Base olarak belirlenmiş olan tüm fonksiyonlara ulaşabilir durumunu barındırmaktadır fakat, bazı tablolar sadece Insert fonksiyonlarına sahip olması gerekirken, Base olarak belirlenmiş olan class'tan miras aldığı için ihtiyacı olmasa bile Update ve Delete fonksiyonlarına da otomatik olarak sahip olmuş olacaktır. Yani;

####	NOT : Kodlar .Net Core EntityFrameworkCore'a göre yazılmıştır.
####	NOT : Fonksiyonlar temsili olarak yazilmistir. Tam anlamiyla calismayabilir.



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

Görüldüğü üzere yukarıda IRepositoryBase<T> generic interfacesi ve RepositoryBase<T> class'ı generic liği ifade etmek isteyen T type için gelecek tüm Context sınıfı (DatabaseContext) içinde ki tablolara denk gelen classlar için burada yazılmış olan tüm fonksiyonlara erişme hakkına sahip olacaklardır. Bu örnek için 
	
###	DatabaseContext sınıfımız;


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

### Tablolar;

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

şeklindedir. Görüldüğü üzere DatabaseContext sınıfımız içerisinde User ve Product adında olan iki tane tablomuz var. IRepositoryBase<T> içerisinde signature olarak tanımlanmış olan ve RepositoryBase<T> class' içerisinde implementation ları yapılan bu fonksiyonlar, RepositoryBase<T> class'ı ve IRepositoryBase<T> interfacesi'nden miras alacak olan tabloların kendilerine ait Repository class'larının üzerinden veya direkt olarak RepositoryBase<T>'den instance üreterek <T> yerine gelen classlar için, o class'ın bu tüm fonksiyonlara ihtiyacı olup olmamasına bakılmadan direkt olarak kullanma imkânına sahip olacaktır. Yani;

.Net Core Console projesi içerisinde olan;

####	Instance yöntemi ->

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

Görüleceği üzere, RepositoryBase<T> den DatabaseContext içerisinde tanımlanmış olan DbSet<TableName> e göre yani Database'imiz içinde olan Tablolarımıza özel bir instance ürettiğimizde, o tablonun Insert, Update veya Delete fonksiyonlarına ihtiyaç olup olmamasına bakılmaksızın direkt olarak intellisense de IRepositoryBase'de tanımlanmış olan ve RepositoryBase'e implementation edilmiş olan tüm fonksiyonlar görülmektedir ve kullanıma açık durumdadır.


####	Miras alan Repository class'ları yöntemi ->

		public interface IRepositoryUser : IRepositoryBase<User>
		{
			//Function signatures
		}

		public class RepositoryUser : RepositoryBase<User>, IRepositoryUser
		{
			//NOT : Eger User tablosuna ait özel bir fonksiyon yazmak istiyorsak bu fonksiyonları IRepositoryBase'e ve RepositoryBase değil
			//IRepositoryUser da signature tanımlaması yapıp (Aynı IRepositoryBase de olduğu gibi) burada implementation yapmalıyız. Böylece 
			//yazılan fonksiyonlar Product için kullanılamaz olmuş olacaktır. Tabloların kendilerine özel Repository classlarinin yazılmasının asıl amacı budur.

			//Function implementations
		}


		public interface IRepositoryProduct : IRepositoryBase<Product>
		{
			//Function signatures
		}

		public class RepositoryProduct : RepositoryBase<Product> , IRepositoryProduct
		{
			//NOT : Eger Product tablosuna ait özel bir fonksiyon yazmak istiyorsak bu fonksiyonları IRepositoryBase'e ve RepositoryBase değil
			//IRepositoryProduct da signature tanımlaması yapıp (Aynı IRepositoryBase de olduğu gibi) burada implementation yapmalıyız. Böylece 
			//yazılan fonksiyonlar User için kullanılamaz olmuş olacaktır. Tabloların kendilerine özel Repository classlarinin yazılmasının asıl amacı budur.

			//Function implementations
		}

		
.Net Core Console projesi içerisinde olan;


#### 	Özel Repository class'ı yöntemi ->

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

Yine görüleceği üzere _UserRepository veya _ProductRepository üzerinden bir fonksiyona ulaşmak istediğimizde intellisense de IRepositoryBase içerisinde tanımlanmış olan ve RepositoryBase içerisinde implementation edilmiş olan tüm fonksiyonlara eksiksiz ulaşmış oluyoruz. 

	
		NOT : Dikkat edilecek olursa eğer, RepositoryUser veya RepositoryProduct interface'leri IRepositoryBase'den miras almasa bile RepositoryUser ve RepositoryProduct class'ları RepositoryBase'den miras aldıkları için IRepositoryBase içinde ki fonksiyonlara ulaşabilme özgürlüğüne sahip olacaktır. Bu durumda Service Oriented Architecture - Interface Segeration mantığına uygun olmayan bir durum oluşacaktır. Interface'lerin oluşturulması ve fonksiyonların signature tanımlamalarının interfacelerde yapılmasının amacı, class'lar kendi başlarına hiçbir işe yaramaz ve interface'ye bağımlıdır. Interfaceler üzerinde yetenek olarak tanımlanmamış olan hiçbir işi yapamaz ve dışarıya açamaz (Class'larin içinde özel olarak yazılmış olan private fonksiyonlar bunlara dahil değildir!) anlamına gelmektedir. 

	
Şimdi bizim yapacak olduğumuz tüm geliştirmeler ve yazımların hepsi, Service Oriented Architecture ve ihtiyacın olanı kullan yani Interface Segeration mantığına uygun hale getirmek içindir. Hiçbir tablo, RepositoryBase içerisinde yazılmış olan ve ihtiyacı olmayan fonksiyona ulaşamaz mantığı ile çalışmayı sağlamaktır.


Bu sorunları çözmek için en hızlı iki türlü çözüm olarak yaklaşım sergileyebiliriz. Daha fazla çözümü belki de bu çözdümden daha iyi bir çözümü siz uygulayabilirsiniz. Bu durumda lütfen projemi fork layarak kendi geliştirmenizi yapıp pull request talebi açınız :-)


###	İlk çözüm ->

			Tabloların ihtiyaçları olabilecek olduğu tüm CRUD fonksiyonlarının kombinasyonu olan 4! tane class oluşturmak. Yani;

		RepositorySelectInsertUpdateDelete<T>, 
		RepositorySelectInsertUpdate<T>, RepositorySelectUpdateDelete<T>,
		RepositorySelectAndInsert<T>, RepositorySelectAndUpdate<T>, RepositorySelectAndDelete<T>, 
		RepositoryUpdateAndInsert<T>, RepositoryUpdateAndDelete<T>, RepositoryInsertAndDelete<T>, ....

şeklinde görüldüğü üzere bu durum uzayıp gidiyor. Bu durumda sizin de tahmin edeceğiniz gibi bu class'ların ve Interface'lerin yönetimi aşırı zor ve kalabalık olacaktır. Bu çözüm bence mantıksız bir çözümdür.


###	İkinci çözüm ->


		Tabloların ihtiyaçları olabilecek olan tüm CRUD fonksiyonlarının parçalanmış halde ki interfacelerinin yazılması. Yani;

		IRepositorySelect<T>, IRepositoryInsert<T>, IRepositoryUpdate<T> ve IRepositoryDelete<T> şeklinde. İçleri şu şekil olmalıdır:

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


C - R - U - D fonksiyonlarımız için temel ihtiyaçlara hizmet verebilecek tüm fonksiyonları içeren interfaceleri hazırladık.Şimdi bu interfacelerin bir yerde implementation edilmesi gerekiyor. Burada dikkat edilmesi gereken şey .Net C# bir class, bir tane abstact veya class tan ve N (N = 999) tane interface'den miras alabilir. Bu yüzden biz bu 4 tane interface için 4 farklı class ta implementation yapmak yerine base yaklaşım olan tek bir class üzerinde implementation işlemini yapacağız fakat tek bir farklılıkla bu implementationları gerçekleştireceğiz. Yani;

	
		NOT : Yukarıda tanımlanmış olan DatabaseContext ile devam edilecektir. User ve Product tablosu üzerinden örnekler gösterilecektir.
		
		
		
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

Dikkat edileceği üzere normalde public olan IRepositoryXXX<T> interfacelerimiz için implementation yaptığımızda normalde tüm fonksiyonların access modifierleri public olması gerekirken burada Interface adı ile implementation yapıldığı için herhangi bir access modifier veremiyoruz. Fakat bu fonksiyonların hepsi public tir. Dikkat edilmesi gereken tek şey direkt olarak bu class'a ulaşmak istersek bu fonksiyonlara private miş gibi ulaşamayız. İşte bu yöntem ile Service Oriented Architecture yaklaşımını tam anlamıyla uygulamış oluyourz.


Şimdi Product ve User classlarımız için tekrar birer tane IRepositoryXXX interfacesi ve RepositoryXXX classı oluşturalım.

		
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

Görüldüğü üzere, normal - temel Generic Repository Design Pattern de olduğu gibi tablolara özel olan IRepositoryXXX ve RepositoryXXX classlarinda herhangi bir değişikliğe gitmiyoruz. Dikkat ederseniz eğer normal - temel yaklaşımdan tek farklı olarak girdiğimiz kısım sadece ve sadece RepositoryBase<T> içinde yazdığımız fonksiyonların signature'lerine müdahale ettik. Haricinde şimdiye kadar değiştirdiğimiz hiçbir şey olmadı. Şimdi bunları böyle yaptık fakat nasıl kullanacağız veya şimdiye kadar yapmış olduğumuz kullanımlardan farklı bize ne katacak derseniz;


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

		
Görüleceği üzere, RepositoryBase<T> içerisinde IRepositoryDeletable<T> interfacesinden gelen DeleteItem fonksiyonu implementation edildiği halde ve bizim RepositoryUser class'ımız RepositoryBase<T>'den (T:User) miras aldığı halde, RepositoryBase<T> içerisinde olan DeleteItem fonksiyonuna _UserRepository instancesinden ulaşamadık fakat _ProductRepository instancesi üzerinden tüm fonksiyonlarımıza ulaşabilmiş olduk. 


Bunun nedeni nedir? dersek ->

Dikkat edilecek olursa 

		IRepositoryUser interfacesi IRepositorySelectable, IRepositoryInsertable ve IRepositoryUpdatable interfacelerinden inherit edilmekteyken;

		IRepositoryProduct interfacesi IRepositorySelectable, IRepositoryInsertable, IRepositoryUpdatable ve IRepositoryDeletable interfacelerinden inherit edilmektedir. 

Bu da bize, IRepositoryUser üzerinden concrete edilmiş olan RepositoryUser class'ından bir instance üretilince IRepositoryDeletable interfacesinde olan fonksiyonlara sahip olsa bile ulaşamama - erişememe imkânı sağlamış olmaktadır. Yani Encapsulation yaptık diyebiliriz.


		NOT : RepositoryUser veya RepositoryProduct classlarından bir instance üretirken eğer bu instancelerin tiplerini direkt kendilerine eşitlersek RepositoryBase üzerinden gelen hiçbir fonksiyonu görme imkânımız olmayacaktır. Service Oriented yaklaşımı sayesinde gerekli Encapsulation işlemini bu şekilde sağlamış oluyoruz. Yani class'ın hizmet verebileceği tek şey, inherit edilmiş olduğu IRepositoryUser veya IRepositoryProduct üzerinden gelecek olan fonksiyonlardır.

            // RepositoryUser _RepositoryOfUser = new RepositoryUser(); -> Bu instance için hiçbir fonksiyona ulaşamayız. Sadece
			// RepositoryUser class'ı object olduğu için object üzerinden gelen temel fonksiyonlar gelir (GetType, Equals, ToString ve GetHashCode)

Tüm Repository ile alakalı olan instanceler, tabloların IRepositoryXXX şeklinde isme sahip interfaceler üzerinden yapılmalıdır.
			

Artık tam anlamıyla bir Service Oriented Architecture ve parçalanmış bir Generic Repository Design Pattern'in çok güzel bir haline ulaşmış olduk. 
		
