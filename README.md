# Software Engineer Technical Test

## Instructions
please update the input and output file locations in appsettings.json in the main project
please update the path in the resources file in the test project

if you don't do this, it will fail.

## Thoughts
I borrowed a design pattern called Object Pipelines and if you pay attention, you will note that the output of each interface is passed on to the next. The design pattern is one of inheritane and uses the same object to act on, obviously I couldn't do this. This is, in essence, an ETL with some business logic behind it.

Unfortunately, due to "laziness", this is not a multi-threaded program. It has a static class! This is purely because it was an exercise. There are MANY ways I could have handled that differently and made it multi-threaded. One possibility, when you need the count of commas, you simply count the number of properties within an InsuranceClaim using reflection. You only need do it once and in one place.

The min and max years could have been calculated ClaimsAccumulator and passed down as a class to the file writer.

The stop in the Worker could be removed, so that it runs continuosly and you could (once multi-threaded) send multiple files and each would get handled. I would have to adjust how the output filename works, i.e. use the input filename and append "-output" to it.

## Notes

This is simply to show, split your code into responsibilities, let each do one thing.
Ensure that your test coverage is sufficient. 

I think that almost every line of code is tested.

Regards
Jai
