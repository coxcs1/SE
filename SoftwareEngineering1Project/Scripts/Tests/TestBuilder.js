/**
* Test Builder using knockout js api.
* Author: Jarred Light
*/

/**
 * This function is a generic exception
 * for the test builder.
 * @param message
 * @constructor
 */
function TestBuilderException(message) {
    this.message = message;
    this.name = 'TestBuilderException';
}


/**
* This class represents a question form element.
*/
function Question(label, fieldName, id, questionOptions) {
    var self = this;
    self.label = label;
    self.fieldName = fieldName;
    self.id = id;
    self.questionOptions = questionOptions;
    //this function selects an option if the question already has a value
    self.selectOption = function (option, item) {
        if (self.id() != null) {
            if (option.value == self.id()) {
                $(option).attr('selected', 'selected');
            }
        }
    };
}

/**
* This class represents a question option.
*/
function QuestionOption(id, text) {
    var self = this;
    self.id = id;
    self.text = text;
}

function TestBuilderViewModel(questionFieldList) {
    var self = this;
        
    self.availableQuestions = ko.observableArray([]);//build an empty array of questions
    //fetch all the questions from the database
    $.get('/questions/getallquestions').done(function (data) {
        $(data).each(function () {
            self.availableQuestions.push(new QuestionOption(this.ID, this.Text));
        });
    });

    //if there are no questions incoming then
    //set up a new test builder
    //else build from passed in data
    if (typeof(questionFieldList) == 'undefined') {
        self.questionFieldList = ko.observableArray([new Question(ko.observable('Question 1'), ko.observable('question1'), ko.observable(null), self.availableQuestions)]);
        var questionCount = 1;//holds place of how many questions there are
    } else {
        self.questionFieldList = questionFieldList;
        var questionCount = self.questionFieldList().length;
        for (var i = 0; i < questionCount; i++) {
            self.questionFieldList()[i].questionOptions = self.availableQuestions;
        }
    }

    //this event listener handles adding question elements
    self.addQuestion = function () { 
        questionCount++;
        self.questionFieldList.push(new Question(ko.observable('Question ' + questionCount), ko.observable('question' + questionCount), ko.observable(null), self.availableQuestions));
    };

    self.removeQuestion = function (question) {
        if (questionCount == 1) {
            toastr.error('Each test must contain at least 1 question');
        } else {
            questionCount--;
            self.questionFieldList.remove(question);
            self.refreshFieldLabelsAndNames();
        }
    };

    //refresh label and field name attributes
    self.refreshFieldLabelsAndNames = function () {
        for (var i = 0; i < self.questionFieldList().length; i++) {
            self.questionFieldList()[i].label('Question ' + (i + 1));
            self.questionFieldList()[i].fieldName('question' + (i + 1));
        }
    };
}