namespace __4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationContext context = new ApplicationContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Student student = new Student()
            {
                Name = "Lucho"
            };

            Course course = new Course()
            {
                Name = "Web"
            };

            student.StudentCourses.Add(new StudentCourse
            {
                Course = course
            });

            // или
            //course.StudentCourses.Add(new StudentCourse
            //{
            //    Student = student
            //}); 


            context.Students.Add(student);

            context.SaveChanges();
        }
    }
}