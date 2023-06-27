using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace __4
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CourseId { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StudentId { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();
    }
}
