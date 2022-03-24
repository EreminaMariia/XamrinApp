using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.Json;
using System.IO;

namespace CalendarApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page : ContentPage
    {
        DateTime[] week;
        int center = 3;

        Color firstDayColor = Color.Salmon;
        Color specialDaysColor = Color.LightSalmon;
        Color pastDaysColor = Color.Red;

        //first day, length
        List<DateTime> redDays = new List<DateTime>
        {
            new DateTime (2022, 1, 19),
            new DateTime (2022, 2, 19),
            new DateTime (2022, 2, 20),
            new DateTime (2022, 2, 21),
            new DateTime (2022, 2, 22)
        };

        int period = 28;

        Dictionary<DateTime, List<string>> tasksDict = new Dictionary<DateTime, List<string>>
        {
            { new DateTime (2022, 3, 19), new List<string>{"Задача19", "Задача19 ещё одна"} },
            { new DateTime (2022, 3, 18), new List<string>{"Задача18", "Задача18 ещё одна"} },
            { new DateTime (2022, 3, 20), new List<string>{"Задача20"} },
            { new DateTime (2022, 3, 21), new List<string>{"Задача21", "Задача21 ещё одна", "Задача21 ещё-ещё" } }
        };

        DateTime first;
        public Page()
        {
            InitializeComponent();
            TodoList();
            week = new DateTime[7];
            redDays.Sort();
            first = FindFirstDay();
            RefreshWeek(DateTime.Today);
            Refresh();

            
        }


        private async Task TodoList()
        {
            using (FileStream fs = new FileStream("ToDo.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<Dictionary<DateTime, List<string>>>(fs, tasksDict);
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            bool parsedDay = int.TryParse(button.Text, out int day);

            if (parsedDay)
            {
                for (int i = 0; i < week.Length; i++)
                {
                    if (week[i].Day == day)
                    {
                        DateTime newDay = week[i];
                        RefreshWeek(newDay);
                        Refresh();
                        break;
                    }
                }
            }
        }

        private async void Add_Click(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrWhiteSpace(taskEntry.Text))
            {
                bool result;
                //возможно переписать логику через first
                
                if (today.BorderColor == specialDaysColor || today.BorderColor == firstDayColor)
                {
                    string alert = "Предупреждаю, это будет мучительно.";
                    if (today.BorderColor == firstDayColor)
                    {
                        alert += "Сегодня вдвойне.";
                    }
                    result = await DisplayAlert("Ты точно этого хочешь?", alert, "Да", "Нет");
                }
                else
                {
                    result = true;
                }

                if (result)
                {
                    if (tasksDict.ContainsKey(week[3]))
                    {
                        tasksDict[week[t]].Add(taskEntry.Text);
                    }
                    else
                    {
                        tasksDict.Add(week[t], new List<string> { taskEntry.Text });                     
                    }
                    taskEntry.Text = "";
                }
            }

            RefreshTasks();
        }

        private DateTime FindFirstDay()
        {
            DateTime firstDay = redDays[0];
            for (int k = redDays.Count - 1; k >= 0; k--)
            {
                if (k == 0)
                {
                    if (redDays[1] != redDays[k].AddDays(1))
                    {
                        firstDay = redDays[k];
                        break;
                    }
                }
                else if (redDays[k - 1] != redDays[k].AddDays(-1))
                {
                    firstDay = redDays[k];
                    break;
                }
            }
            return firstDay;
        }



        private void RefreshWeek(DateTime day)
        {
            for (int i = 0; i < week.Length; i++)
            {
                week[i] = day.AddDays(i - 3);
            }
        }

        private void Refresh()
        {
            for (int i = 0; i < week.Length; i++)
            {
                Button b = (Button)grid.Children[i];
                int d = week[i].Day;
                b.Text = d > 9 ? d.ToString() : "0" + d.ToString();
                if (week[i] > first.AddDays(10))
                {
                    TimeSpan diff = week[i] - first;
                    int diffDays = diff.Days;
                    int divrem = diffDays % period;
                    b.BorderColor = divrem < 6 ? (divrem == 0 ? firstDayColor : specialDaysColor) : new Color(255, 255, 255);
                }
                else
                {
                    b.BorderColor = redDays.Contains(week[i]) ? pastDaysColor : new Color(255, 255, 255);
                }
            }
            int m = week[t].Month;
            MonthNames name = (MonthNames)m;
            month.Text = name.ToString();
            year.Text = week[t].Year.ToString();

            add_button.IsEnabled = week[t] > DateTime.Today;
            taskEntry.IsEnabled = week[t] > DateTime.Today;

            RefreshTasks();
        }

        private void RefreshTasks()
        {
            tasks.Children.Clear();
            if (tasksDict.ContainsKey(week[t]))
            {
                List<string> todayTasks = tasksDict[week[t]];

                for (int i = 0; i < todayTasks.Count; i++)
                {
                    Label label = new Label
                    {
                        Text = todayTasks[i],
                    };
                    tasks.Children.Add(label);
                }
            }
        }
    }

    public enum MonthNames
    {
        Январь = 1,
        Февраль = 2,
        Март = 3,
        Апрель = 4,
        Май = 5,
        Июнь = 6,
        Июль = 7,
        Август = 8,
        Сентябрь = 9,
        Октябрь = 10,
        Ноябрь = 11,
        Декабрь = 12
    }
}