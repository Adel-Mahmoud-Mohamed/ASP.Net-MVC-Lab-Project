use itialex46;

INSERT INTO Departments (DeptId, DeptName, Capacity) VALUES
(1, 'Computer Science', 200),
(2, 'Information Systems', 150),
(3, 'Business Administration', 180),
(4, 'Engineering', 220);


INSERT INTO Students (Name, Age, Email, Deptno) VALUES
('Ahmed Hassan', 21, 'ahmed.hassan@mail.com', 1),
('Mona Ali', 22, 'mona.ali@mail.com', 2),
('Omar Khaled', 20, 'omar.khaled@mail.com', 1),
('Sara Mohamed', 23, 'sara.m@mail.com', 3),
('Youssef Adel', 21, 'youssef.adel@mail.com', 4);

INSERT INTO Courses (CrsId, Name, Duration) VALUES
(101, 'Database Systems', 60),
(102, 'Data Structures', 75),
(103, 'Operating Systems', 80),
(104, 'Marketing Principles', 50),
(105, 'Network Fundamentals', 65);

INSERT INTO CourseDepartment (CoursesCrsId, DepartmentsDeptId) VALUES
(101, 1),
(102, 1),
(103, 1),
(105, 1),

(101, 2),
(105, 2),

(104, 3),

(103, 4),
(105, 4);


INSERT INTO StudentCourses (StudentId, CrsNo, Degree) VALUES
(1, 101, 85),
(1, 102, 90),
(1, 105, 88),

(2, 101, 75),
(2, 105, 80),

(3, 102, 92),
(3, 103, 78),

(4, 104, 89),

(5, 103, 70),
(5, 105, 73);
