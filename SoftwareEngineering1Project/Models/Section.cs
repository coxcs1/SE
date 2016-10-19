///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Section.cs
//	Description:       This class represents a Section beloning to a course being taught by a teacher.
//
//	Author:            Dana Jarred Light, lightdj@etsu.edu
//
///////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Models
{
    public class Section
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public int ID { get; set; }

        public enum Semesters
        {
            Fall,
            Spring,
            Summer
        }

        /// <summary>
        /// Gets or sets the semester.
        /// </summary>
        /// <value>
        /// The semester.
        /// </value>
        public Semesters Semester { get; set; }

        /// <summary>
        /// Gets or sets the academic year.
        /// </summary>
        /// <value>
        /// The academic year.
        /// </value>
        public int AcademicYear { get; set; }
        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        /// <value>
        /// The course identifier.
        /// </value>
        [ForeignKey("Course")]
        public int CourseID { get; set; }

        /// <summary>
        /// Gets or sets the teacher identifier.
        /// </summary>
        /// <value>
        /// The teacher identifier.
        /// </value>
        [ForeignKey("Teacher")]
        public int TeacherID { get; set; }

        /// <summary>
        /// Gets or sets the teacher.
        /// </summary>
        /// <value>
        /// The teacher.
        /// </value>
        public virtual Teacher Teacher { get; set; }

        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        /// <value>
        /// The course.
        /// </value>
        public virtual Course Course { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions added to this course.
        /// </value>
        public virtual ICollection<Question> Questions { get; set; }
    }
}