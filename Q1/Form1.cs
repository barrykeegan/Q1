using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace Q1
{
    public partial class Form1 : Form
    {
        List<WaterMeter> WaterMeterRecords;
        List<Account> AccountRecords;

        public Form1()
        {
            InitializeComponent();
            List<string> csvWaterMeters = new List<string>()
            {
                "100,68405,0,10001",
                "101,11369,1,10002",
                "102,138115,0,10003",
                "103,102191,1,10004",
                "104,791007,0,10005"
            };
            /*WaterMeterRecords = new List<WaterMeter>() {
                new WaterMeter
                {
                    MetreID = 100,
                    VolumeUsed = 68405,
                    HasBeenServiced = false,
                    OwnerAccountID = 10001
                },
                new WaterMeter
                {
                    MetreID = 101,
                    VolumeUsed = 11369,
                    HasBeenServiced = true,
                    OwnerAccountID = 10002
                },
                new WaterMeter
                {
                    MetreID = 102,
                    VolumeUsed = 138115,
                    HasBeenServiced = false,
                    OwnerAccountID = 10003
                },
                new WaterMeter
                {
                    MetreID = 103,
                    VolumeUsed = 102191,
                    HasBeenServiced = true,
                    OwnerAccountID = 10004
                },
                new WaterMeter
                {
                    MetreID = 104,
                    VolumeUsed = 791007,
                    HasBeenServiced = false,
                    OwnerAccountID = 10005
                }
            };*/
            List<string> csvAccountRecords = new List<string>()
            {
                $"10001,K64R102,{(int)PaymentPeriod.Annual},2",
                $"10002,K64R103,{(int)PaymentPeriod.Monthly},2",
                $"10003,K64R104,{(int)PaymentPeriod.Quaterly},1",
                $"10004,K64R101,{(int)PaymentPeriod.Monthly},0",
                $"10005,K64R102,{(int)PaymentPeriod.BiAnnual},0",
                $"10006,K64R103,{(int)PaymentPeriod.BiAnnual},0"
            };
            /*AccountRecords = new List<Account>()
            {
                new Account {AccountID = 10001, EirCode = "K64R102", PaymentPeriod = PaymentPeriod.Annual, ArrearsCount = 2 },
                new Account {AccountID = 10002, EirCode = "K64R103", PaymentPeriod = PaymentPeriod.Monthly, ArrearsCount = 2},
                new Account {AccountID = 10003, EirCode = "K64R104", PaymentPeriod = PaymentPeriod.Quaterly, ArrearsCount = 1},
                new Account {AccountID = 10004, EirCode = "K64R101", PaymentPeriod = PaymentPeriod.Monthly, ArrearsCount = 0},
                new Account {AccountID = 10005, EirCode = "K64R102", PaymentPeriod = PaymentPeriod.BiAnnual, ArrearsCount = 0},
                new Account {AccountID = 10006, EirCode = "K64R103", PaymentPeriod = PaymentPeriod.BiAnnual, ArrearsCount = 0}
            };*/
            if (!File.Exists(System.Configuration.ConfigurationManager.AppSettings["metersfile"]))
            {
                using (FileStream fstream = File.Create(ConfigurationManager.AppSettings["metersfile"]))
                /*
                Observe the explicit use of the encoding enum value set to UTF8--it is 
                always best to be explicit in your code and not rely on default class
                values/behaviours.
                */
                using (TextWriter txtWriter = new StreamWriter(fstream, Encoding.UTF8))
                {
                    foreach (string line in csvWaterMeters)
                    {
                        txtWriter.WriteLine(line);
                    }
                }
            }

            if (!File.Exists(System.Configuration.ConfigurationManager.AppSettings["accountsfile"]))
            {
                using (FileStream fstream = File.Create(ConfigurationManager.AppSettings["accountsfile"]))
                /*
                Observe the explicit use of the encoding enum value set to UTF8--it is 
                always best to be explicit in your code and not rely on default class
                values/behaviours.
                */
                using (TextWriter txtWriter = new StreamWriter(fstream, Encoding.UTF8))
                {
                    foreach (string line in csvAccountRecords)
                    {
                        txtWriter.WriteLine(line);
                    }
                }
            }

            if (File.Exists(System.Configuration.ConfigurationManager.AppSettings["accountsfile"]))
            {
                using (FileStream fStream = File.OpenRead(ConfigurationManager.AppSettings["accountsfile"]))
                using (TextReader txtReader = new StreamReader(fStream, Encoding.UTF8))
                {
                    string line = null;
                    string[] accountRecord = null;
                    AccountRecords = new List<Account>();
                    while (txtReader.Peek() > -1)
                    {
                        line = txtReader.ReadLine();
                        accountRecord = line.Split(',');

                        AccountRecords.Add(
                            new Account
                            {
                                AccountID = int.Parse(accountRecord[0]),
                                EirCode = accountRecord[1],
                                PaymentPeriod = (PaymentPeriod)int.Parse(accountRecord[2]),
                                ArrearsCount = int.Parse(accountRecord[3])
                            });

                    }
                }
            }

            if (File.Exists(System.Configuration.ConfigurationManager.AppSettings["metersfile"]))
            {
                using (FileStream fStream = File.OpenRead(ConfigurationManager.AppSettings["metersfile"]))
                using (TextReader txtReader = new StreamReader(fStream, Encoding.UTF8))
                {
                    string line = null;
                    string [] meterRecord = null;
                    WaterMeterRecords = new List<WaterMeter>();
                    while (txtReader.Peek() > -1)
                    {
                        line = txtReader.ReadLine();
                        meterRecord = line.Split(',');
                        
                        WaterMeterRecords.Add(
                            new WaterMeter
                            {
                                MetreID = int.Parse(meterRecord[0]),
                                VolumeUsed = int.Parse(meterRecord[1]),
                                HasBeenServiced = meterRecord[2] == "1",
                                OwnerAccountID = int.Parse(meterRecord[3])
                            });
                        
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvWaterMeters.DataSource = (from waterMeter in WaterMeterRecords
                                         select waterMeter).ToList();

            dgvAccount.DataSource = null;
        }

        private void dgvWaterMeters_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int selectedAccountID = Convert.ToInt32(dgvWaterMeters.Rows[e.RowIndex].Cells[3].Value);

                dgvAccount.DataSource = (from account in AccountRecords
                                         where account.AccountID == selectedAccountID
                                         select account).ToList();
            }
        }

        private void rbMetreID_CheckedChanged(object sender, EventArgs e)
        {
            if(rbMetreID.Checked)
            {
                dgvWaterMeters.DataSource = (from waterMeter in WaterMeterRecords
                                             orderby waterMeter.MetreID
                                             select waterMeter).ToList();

                dgvAccount.DataSource = null;
            }
        }

        private void rbVolumeUsed_CheckedChanged(object sender, EventArgs e)
        {
            dgvWaterMeters.DataSource = (from WaterMeter in WaterMeterRecords
                                         orderby WaterMeter.VolumeUsed
                                         select WaterMeter).ToList();

            dgvAccount.DataSource = null;
        }

        private void btnCustomerArrears_Click(object sender, EventArgs e)
        {
            using (CustomerArrearsReport custAR = new CustomerArrearsReport(AccountRecords))
            {
                custAR.ShowDialog();
            }
        }

        private void btnSummaryReport_Click(object sender, EventArgs e)
        {
            int totalVolWaterUsed = (from waterMeter in WaterMeterRecords
                                     select waterMeter.VolumeUsed).Sum();

            int totalAccountsInArrears = (from account in AccountRecords
                                          where account.ArrearsCount > 0
                                          select account.ArrearsCount).Count();

            string msg = $"Total volume of water used = {totalVolWaterUsed} & total number of accounts in arrears is {totalAccountsInArrears}";
            MessageBox.Show(msg);
        }
    }
}

/*FURTHER LINQ EXAMPLES
 * ************************************************************************
 * 
To write LINQ queries we use the LINQ Standard Query Operators. The following are a few Examples of Standard Query Operators
select
from
where 
orderby 
join
groupby

LINQ query using Lambda Expressions.
IEnumerable[Student] students = Student.GetAllStudents().Where(student =] student.Gender == "Male");

LINQ query using using SQL like query expressions
IEnumerable[Student] students = from student in Student.GetAllStudents()
                                where student.Gender == "Male"
                                select student;

To bind the results of this LINQ query to a GridView
GridView1.DataSource = students;
GridView1.DataBind();

GroupBy operator belong to Grouping Operators category. This operator takes a flat sequence of items, organize that sequence into groups (IGrouping[K,V]) based on a specific key and return groups of sequences. 

In short, GroupBy creates and returns a sequence of IGrouping[K,V]

Example 1: Get Employee Count By Department
var employeeGroup = from employee in Employee.GetAllEmployees()
                    group employee by employee.Department;

foreach (var group in employeeGroup)
{
    Console.WriteLine("{0} - {1}", group.Key, group.Count());
}

Example 2: Get Employee Count By Department and also each employee and department name
var employeeGroup = from employee in Employee.GetAllEmployees()
                    group employee by employee.Department;

foreach (var group in employeeGroup)
{
    Console.WriteLine("{0} - {1}", group.Key, group.Count());
    Console.WriteLine("----------");
    foreach (var employee in group)
    {
        Console.WriteLine(employee.Name + "\t" + employee.Department);
    }
    Console.WriteLine(); Console.WriteLine();
}

Example 3: Get Employee Count By Department and also each employee and department name. Data should be sorted first by Department in ascending order and then by Employee Name in ascending order.
var employeeGroup = from employee in Employee.GetAllEmployees()
                    group employee by employee.Department into eGroup
                    orderby eGroup.Key
                    select new { Key = eGroup.Key, Employees = eGroup.OrderBy(x =] x.Name) };

foreach (var group in employeeGroup)
{
    Console.WriteLine("{0} - {1}", group.Key, group.Employees.Count());
    Console.WriteLine("----------");
    foreach (var employee in group.Employees)
    {
        Console.WriteLine(employee.Name + "\t" + employee.Department);
    }
    Console.WriteLine(); Console.WriteLine();
}.

Example 1: Group employees by Department and then by Gender. The employee groups should be sorted first by Department and then by Gender in ascending order. Also, employees within each group must be sorted in ascending order by Name.
var employeeGroups = Employee.GetAllEmployees()
                    .GroupBy(x =] new { x.Department, x.Gender })
                    .OrderBy(g =] g.Key.Department).ThenBy(g =] g.Key.Gender)
                    .Select(g =] new
                    {
                        Dept = g.Key.Department,
                        Gender = g.Key.Gender,
                        Employees = g.OrderBy(x =] x.Name)
                    });

Example 2: Rewrite Example 1 using SQL like syntax
var employeeGroups = from employee in Employee.GetAllEmployees()
                     group employee by new { employee.Department, employee.Gender } into eGroup
                     orderby eGroup.Key.Department ascending, eGroup.Key.Gender ascending
                     select new
                     {
                        Dept = eGroup.Key.Department,
                        Gender = eGroup.Key.Gender,
                        Employees = eGroup.OrderBy(x =] x.Name)
                     };


 * ************************************************************************/
