# Nancy.Rest.Client

Dynamic proxy client generation for Nancy.

## Prerequisites

A server using Nancy & Nancy.Rest.Module.

You should read Nancy.Rest.Module documentation to understand how it works before continuing reading.

## Installation

Add Nancy.Rest.Client and Nancy.Rest.Annotations to your client project.

Add server models and the interface with the method signatures to use.

## Basic Usage

###Your Server signatures:

`
namespace Nancy.Rest.ExampleServer
{
    [RestBasePath("/")]
    public interface IExample
    {

        [Rest("Person", Verbs.Get)]
        List<Person> GetAllPersons();

        [Rest("Person/{personid}", Verbs.Get)]
        Person GetPerson(int personid);
        
        [Rest("Person", Verbs.Post)]
        bool SavePerson(Person person);

        [Rest("Person/{personid}", Verbs.Delete)]
        bool DeletePerson(int personid);
    }
}
`

###Your Server Models

`

namespace Nancy.Rest.ExampleServer
{    
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public bool IsProgrammer { get; set; }
        
        public List<string> Tags { get; set; }
    }
}

`

#### Your Client

`
namespace Nancy.Rest.ExampleClient
{    
    public class Example
    {
        public void Run()
        {
            IExample server=ClientFactory.Create<IExample>("http://yourserver/api"); `
        
            List<Person> persons=server.GetPersons();            
            
        }
    }
}


##Advanced Usage

TODO

## History

**1.4.3-Alpha**: First Release

## Built With

* [JSON.Net](newtonsoft.com/json/) 
* [RestSharp](http://restsharp.org/)

## Credits

* **Maximo Piva** -[maxpiva](https://github.com/maxpiva)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details


