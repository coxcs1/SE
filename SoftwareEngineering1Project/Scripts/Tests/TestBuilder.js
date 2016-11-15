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
 * This function represents a step in the testing process.
 * @param id
 * @param name
 * @param template
 * @param questions
 * @param hidden
 * @constructor
 */
function Step(id, name, template, questions, hidden) {
    var self = this;
    self.id = id;
    self.name = ko.observable(name);
    self.template = template;
    self.questions = questions;
    self.hidden = ko.observable(hidden);
    self.questionCount = 1;

    self.getTemplate = function () {
        return self.template;
    };

    /**
     * This function displays new questions every time you click the Next Question button.
     * @throws TestBuilderException
     */
    self.nextQuestion = function () {
        try{
            if (self.questionCount < self.questions().length) {
                self.questions()[self.questionCount].hidden(true);
                self.questionCount++;
            } else {
                throw new TestBuilderException('Question limit reached');
            }
        } catch (exception) {
            toastr.error(exception.message);
        }
    };

    /**
     * This function will grab a new question from the server and replace the data.
     * @param question
     */
    self.generateNewQuestion = function (question) {
        question.text("New Question");
        question.answer("New Answer");
        question.score(0);
        //TODO: Create an AJAX call to fetch a new question
    };
}

/**
 * This object represents a question and is used in the view model.
 * @param label
 * @param fieldName
 * @param text
 * @param answer
 * @param score
 * @param hidden
 * @constructor
 */
function Question(id, label, fieldName, text, answer, score, hidden) {
    var self = this;

    self.id = id;
    self.label = ko.observable(label);
    self.fieldName = ko.observable(fieldName);
    self.text = ko.observable(text);
    self.answer = ko.observable(answer);
    self.score = ko.observable(score);
    self.hidden = ko.observable(hidden);

    /**
     * This function saves the score of a question.
     * @param question
     */
    self.saveScore = function (question) {
        var score = question.score();
        console.log(score);
        //TODO: Create an AJAX post request to save the score in the database
        return true;
    };
}

/**
 * This function builds an array of questions sent from the server.
 * @param questions
 */
function getQuestions(questions) {
    var questionsArray = ko.observableArray([]);
    var count = 0;
    $.each(questions, function () {
        questionsArray.push(new Question(this.ID, this.Label, this.FieldName, this.Text, this.Answer, this.Score, (count == 0)));
        count++;
    });
    return questionsArray;
}

/**
 * This function builds an array of steps sent from the server.
 * @param model
 */
function getSteps(model) {
    var steps = ko.observableArray([]);
    var count = 1;
    $.each(model, function () {
        steps.push(new Step(count, "Section: " + this.Section, "step" + count, getQuestions(this.Questions), (count == 1)));
        count++;
    });
    return steps;
}

/**
 * This function contains all the necessary functions and member data for
 * the testing process.
 * @param model
 * @constructor
 */
function TestBuilderViewModel(model) {
    var self = this;//adjust the context

    self.currentStep = 0;//set initial step to the first one
    self.steps = getSteps(model);//fetch all steps from the json data
    self.submitVisible = ko.observable(false);//hide the final submit button
    self.previousVisible = ko.observable(false);//hide the previous button
    self.nextVisible = ko.observable(true);//display the next button

    //if it's the last slide then show the final submit
    if (self.currentStep == self.steps().length - 1) {
        self.nextVisible(false);
        self.submitVisible(true);
    }

    /**
     * This function goes to the next section of the test if there are any more.
     * @throws TestBuilderException
     */
    self.nextSection = function () {
        var currentStep = self.steps()[self.currentStep];//set the current step
        //check to see if any questions haven't been administered
        try{
            $.each(currentStep.questions(), function () {
                if (!this.hidden()) {
                    throw new TestBuilderException(this.label() + ' has not been administered');
                }
                if (this.score() < 1) {
                    throw new TestBuilderException(this.label() + ' has not been scored')
                }
            });

            currentStep.hidden(false);//hide the current step
            self.currentStep++;//increment the step counter
            self.steps()[self.currentStep].hidden(true);//show the next step
            self.previousVisible(true);//show the previous button when on a slide other then the first
            //if it's the last slide then show the final submit
            if (self.currentStep == self.steps().length - 1) {
                self.nextVisible(false);
                self.submitVisible(true);
            }
        } catch (exception) {
            toastr.error(exception.message);
        }
        
    };

    /**
     * This function loads a previous page and sets visible conditions on buttons.
     */
    self.previousSection = function () {
        var currentStep = self.steps()[self.currentStep];//get the current step
        currentStep.hidden(false);//hide the current step
        self.currentStep--;//increment the step counter
        self.steps()[self.currentStep].hidden(true);//show the next step
        //if the step is the first step then hide the previous and submit buttons
        if (self.currentStep == 0) {
            self.previousVisible(false);
            self.submitVisible(false);
            self.nextVisible(true);
        }
    };

    self.finalSubmission = function () {
        var currentStep = self.steps()[self.currentStep];//set the current step
        //check to see if any questions haven't been administered
        try {
            $.each(currentStep.questions(), function () {
                if (!this.hidden()) {
                    throw new TestBuilderException(this.label() + ' has not been administered');
                }
                if (this.score() < 1) {
                    throw new TestBuilderException(this.label() + ' has not been scored')
                }
            });
            
        } catch (exception) {
            toastr.error(exception.message);
        }

        console.log(ko.toJSON(this));
        //TODO: Send this data to the back end for processing - maybe have a module for selecting if the 
        //student has passed the exam or not 
    };
}