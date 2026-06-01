using System.ComponentModel.DataAnnotations;
public enum Category
{
    [Display(Name = "General Public")]
    General = 1,

    [Display(Name = "Defense Attorney")]
    DefenseAttorney = 2,

    [Display(Name = "Educator")]
    Educator = 3,

    [Display(Name = "Student")]
    Student = 4,

    [Display(Name = "Legal Professional")]
    LawProfessional = 5,

    [Display(Name = "Police Officer")]
    Police = 6,

    [Display(Name = "Medical Practitioner")]
    PracticeInMedicine = 7,

    [Display(Name = "Psychologist")]
    Psychologist = 8,

    [Display(Name = "Public Prosecutor")]
    PublicProsecutors = 9,

    [Display(Name = "Social Activist")]
    SocialActivist = 10,

    [Display(Name = "Sociologist")]
    Sociologist = 11,

    [Display(Name = "Law Student")]
    LawStudent = 12,

    [Display(Name = "Law Professor")]
    LawProfessor = 13,

    [Display(Name = "Professor")]
    Professor = 14
}