/**
* This file instantiates the Test Builder.
* Author: Jarred Light
*/
$(document).ready(function () {
    var viewModelData = {
        "Step 1": {
            "Section"
        :
            "Software Design",
            "Questions"
    :
        [
            {
                "Label": "Question 1",
                "FieldName": "question1",
                "Text": "What are two primary types of crust?",
                "Answer": "Continental and Oceanic",
                "Score": "0"
            },
            {
                "Label": "Question 2",
                "FieldName": "question2",
                "Text": "The Earth’s plates are composed of the crust plus part of the upper mantle. What is this called?",
                "Answer": "The lithospehere.",
                "Score": "0"
            },
            {
                "Label": "Question 3",
                "FieldName": "question3",
                "Text": "Where does most oceanic crust come from? ",
                "Answer": "Divergent Boundaries",
                "Score": "0"
            },
            {
                "Label": "Question 4",
                "FieldName": "question4",
                "Text": "What is an ophiolite and what does it indicate if you find one on a continental plate? ",
                "Answer": "Pieces of oceanic crust and upper mantle exposed withing continental rocks. Indicates a subduction zone.",
                "Score": "0"
            },
            {
                "Label": "Question 5",
                "FieldName": "question5",
                "Text": "What is the difference between an active margin and a passive margin?",
                "Answer": "Active margins develop at the edge of a continental plate and form trenches(narrow). Passive margins are wide and built up from sedimentation.",
                "Score": "0"
            }
        ]
        },
        "Step 2": {
            "Section"
                :
                "Software Engineering",
            "Questions"
                :
                [
                    {
                        "Label": "Question 1",
                        "FieldName": "question1",
                        "Text": "What is an oceanic trench and where do these form? ",
                        "Answer": "Long, deep depressions associated with subducting lithosphere",
                        "Score": "0"
                    },
                    {
                        "Label": "Question 2",
                        "FieldName": "question2",
                        "Text": "How did the Hawaiin Islands form?",
                        "Answer": "The Pacfic plate passing over a hotspot in the lithosphere.",
                        "Score": "0"
                    },
                    {
                        "Label": "Question 3",
                        "FieldName": "question3",
                        "Text": "The 'southern' continents (South America, Africa, Antarctica, India, Australia) were once joined. Together they are known as what? ",
                        "Answer": "Gondwana",
                        "Score": "0"
                    },
                    {
                        "Label": "Question 4",
                        "FieldName": "question4",
                        "Text": "What are the three primary sedimentary facies that occur along coastal margins as one moves away from the continent? What can these facies tell us about sea level changes, regressions and transgressions?",
                        "Answer": "Carbonate, mud, and sand facies. That seas depoit minerals on continents and take minerals from continents.",
                        "Score": "0"
                    },
                    {
                        "Label": "Question 5",
                        "FieldName": "question5",
                        "Text": "What is Tiktaalik and why is it an important find? ",
                        "Answer": "Intermediate from between fish and amphibians.",
                        "Score": "0"
                    }
                ]
        }
    };

    //initialize view model
    ko.applyBindings(new TestBuilderViewModel(viewModelData));
});