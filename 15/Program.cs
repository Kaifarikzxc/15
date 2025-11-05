
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Демонстрация: сначала версия с ошибкой, затем исправленная ===\n");

        try
        {
            RunProgramWithError();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n‼️ Произошла ошибка: {ex.GetType().Name} — {ex.Message}");
            Console.WriteLine("Причина: некорректное условие цикла (<= вместо <) при обращении по индексу.");
        }

        Console.WriteLine("\nХотите запустить исправленную версию программы?");
        Console.Write("Введите '1' для продолжения или любую другую клавишу для выхода: ");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.WriteLine("\n=== Исправленная версия программы ===\n");
            RunFixedProgram();
        }
        else
        {
            Console.WriteLine("\nРабота программы завершена.");
        }
    }

    //версия — с ошибкой
    static void RunProgramWithError()
    {
        var students = CreateSampleStudents();

        var gp = new GradeProcessorWithError();
        Student top = gp.FindTopStudent(students);

        // В этой демонстрации мы ожидаем, что программа упадёт в CalculateAverage
   
        if (top == null)
        {
            Console.WriteLine("Не удалось определить лучшего студента.");
            return;
        }

        Console.WriteLine($"Лучший студент: {top.Name}");
    }

    // Исправленная версия
    static void RunFixedProgram()
    {
        var students = CreateSampleStudents();

        var gp = new GradeProcessorFixed();
        Student top = gp.FindTopStudent(students);

        if (top == null)
        {
            Console.WriteLine("Список студентов пуст или нет допустимых данных.");
            return;
        }

        Console.WriteLine($"Лучший студент: {top.Name}");
        Console.WriteLine($"Средний балл: {gp.CalculateAverage(top):F2}");
    }

    static List<Student> CreateSampleStudents()
    {
        return new List<Student>
        {
            new Student("Иванов", new List<int>{5,4,5}),
            new Student("Петров", new List<int>{4,3,4}),
            new Student("Сидоров", new List<int>{5,5,5})
        };
    }
}

// =============================
// Класс Student (использует nullable-аннотации)
class Student
{
    public string Name { get; set; }
    public List<int> Grades { get; set; }

    public Student(string name, List<int>  grades)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Grades = grades ?? new List<int>();
    }
}

// Ошибочная версия GradeProcessor (демонстрация IndexOutOfRange)
class GradeProcessorWithError
{
    public double CalculateAverage(Student s)
    {
        // намеренно не проверяем s.Grades.Count, чтобы показать сбой
        int sum = 0;
        // ОШИБКА: <= вместо < => IndexOutOfRangeException при i == s.Grades.Count
        for (int i = 0; i <= s.Grades.Count; i++)
        {
            sum += s.Grades[i];
        }

        // если Grades.Count == 0 — здесь будет деление на ноль; это часть демонстрации "небезопасного" кода
        return (double)sum / s.Grades.Count;
    }

    public Student FindTopStudent(List<Student> students)
    {
        if (students == null || students.Count == 0)
            return null;

        Student top = null;
        double best = double.MinValue;

        foreach (var st in students)
        {
            double avg = CalculateAverage(st); // тут и произойдет исключение
            if (avg > best)
            {
                best = avg;
                top = st;
            }
        }
        return top;
    }
}

// =============================
// ✅ Исправленная версия GradeProcessor (безопасно)
class GradeProcessorFixed
{
    public double CalculateAverage(Student s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        if (s.Grades == null || s.Grades.Count == 0)
            return 0.0;

        int sum = s.Grades.Sum();
        return (double)sum / s.Grades.Count;
    }

    public Student FindTopStudent(List<Student> students)
    {
        if (students == null || students.Count == 0)
            return null;

                Student top = null;
        double best = double.MinValue;

        foreach (var st in students)
        {
            double avg = CalculateAverage(st);
            if (avg > best)
            {
                best = avg;
                top = st;
            }
        }
        return top;
    }
}
