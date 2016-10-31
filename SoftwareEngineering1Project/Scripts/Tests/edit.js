/**
* This file instantiates the Test Builder and passes populate data.
* Author: Jarred Light
*/
$(document).ready(function () {
    //fetch test questions for this test
    $.get('/tests/gettestquestions/' + TestID).done(function (data) {
        var questionFieldList = ko.observableArray([]);
        for (var i = 0; i < data.length; i++) {
            console.log(data[i].ID);
            var question = new Question(ko.observable('Question ' + (i + 1)), ko.observable('question' + (i + 1)), ko.observable(data[i].ID));
            questionFieldList.push(question);
        }
        ko.applyBindings(TestBuilderViewModel(questionFieldList));//pass the questions field list to the view model
    });
});