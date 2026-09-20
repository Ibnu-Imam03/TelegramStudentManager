using System;
using System.Data;
using System.Collections.Generic;
using TelegramStudentManager.DataAccess;

namespace TelegramStudentManager.Business
{
    public class clsStudent
    {
        private enum _enMode { AddNew = 0, Update = 1 }
        private _enMode _Mode;
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public string StudentNumber { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public string Phone { get; set; }

        public clsStudent()
        {
            this.StudentID = -1;
            this.FullName = "";
            this.StudentNumber = "";
            this.Department = "";
            this.Year = -1;
            this.Phone = "";

            _Mode = _enMode.AddNew;
        }

        private clsStudent(int StudentID, string FullName , string StudentNumber , string Department , int Year , string Phone)
        {
            this.StudentID = StudentID;
            this.FullName = FullName;
            this.StudentNumber = StudentNumber;
            this.Department = Department;
            this.Year = Year;
            this.Phone = Phone;

            _Mode = _enMode.Update;
        }

        public static List<clsStudent> GetAllStudents()
        {
            DataTable dt = clsStudentData.GetAllStudents();

            List<clsStudent> Students = new List<clsStudent>();

            foreach (DataRow row in dt.Rows)
            {
                clsStudent Student = new clsStudent();

                Student.StudentID = Convert.ToInt32(row["Id"]);
                Student.FullName = row["FullName"].ToString();
                Student.StudentNumber = row["StudentNumber"].ToString();
                Student.Department = row["Department"].ToString();
                Student.Year = Convert.ToInt32(row["Year"]);
                Student.Phone = row["Phone"].ToString();

                Students.Add(Student);
            }

            return Students;
        }
        public static clsStudent FindStudentByID (int StudentID)
        {
         string FullName = "";
         string StudentNumber = "";
         string Department = "";
         int Year = -1;
         string Phone = "";

            if (clsStudentData.GetStudentByID(StudentID, ref FullName, ref StudentNumber, ref Department , ref Year , ref Phone))
            {
                return new clsStudent(StudentID, FullName, StudentNumber, Department, Year, Phone);
            }
            else
            {
                return null;
            }
        }

        private  bool _AddNewStudent()
        {
            this.StudentID = clsStudentData.AddStudent(FullName, StudentNumber, Department, Year,Phone);

            return this.StudentID != -1;
        }

        private bool _UpdateStudent()
        {
            return clsStudentData.UpdateStudent(this.StudentID,this.FullName, this.StudentNumber, this.Department, this.Year, this.Phone);
        }

        public static bool DeleteStudent(int StudentID)
        {
            return clsStudentData.DeleteStudent(StudentID);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case _enMode.AddNew:
                    {
                        if (_AddNewStudent())
                        {
                            _Mode = _enMode.Update;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case _enMode.Update:
                    {
                        return _UpdateStudent();
                    }
            }
            return false;
        }


    }
}
