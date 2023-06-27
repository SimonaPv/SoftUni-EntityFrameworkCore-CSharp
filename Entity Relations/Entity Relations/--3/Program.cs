namespace __3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationContext context = new ApplicationContext();

            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

           //Department department = new Department()
           //{
           //    Name = "IT",
           //};
           //
           //Employee employee = new Employee()
           //{
           //    Name = "Gosho",
           //    Department = department
           //};

            //Department department = new Department()
            //{
            //    Name = "Marketing"
            //};
            
            Employee employee = new Employee()
            {
                Name = "Pesho",
                DepartmentId = 1
            };

            context.Employees.Add(employee);

            context.SaveChanges();
        }
    }
}