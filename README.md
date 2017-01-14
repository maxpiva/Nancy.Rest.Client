# Nancy.Rest.Client

Dynamic proxy client generation for Nancy.

## Prerequisites

A server using Nancy & Nancy.Rest.Module.

You should read Nancy.Rest.Module documentation to understand how it works before continuing reading.

## Installation

Add Nancy.Rest.Client and Nancy.Rest.Annotations to your client project.

Add server models and the interface with the method signatures to use.

## Basic Usage

####Your Server signatures:


```csharp

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
```

###Your Server Models

```csharp

namespace Nancy.Rest.ExampleServer
{    
    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        [Level(1)]
        public bool IsProgrammer { get; set; }
        
        [Tags("Attr")]
        public List<string> Attributes { get; set; }
    }
}

```

#### Your Client

```csharp

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

```

##Advanced Usage

###Transversal Filtering

Nancy.Rest.Client includes this interface.

```csharp

using System.Collections.Generic;

namespace Nancy.Rest.Annotations.Interfaces
{
    public interface IFilter<T>
    {
        T FilterWithLevel(int level);

        T FilterWithTags(IEnumerable<string> tags);

        T FilterWithLevelAndTags(int level, IEnumerable<string> tags);

    }
}

```

Create a new interface in your client that includes, both, `IFilter` interface and your server interface.

```csharp

namespace Nancy.Rest.ExampleClient
{    
    public interface IExampleWithFiltering : IExample, IFilter<IExample>
    {
    }
}

```

then you can use the transversal filtering capabilities of the server like this:


```csharp

namespace Nancy.Rest.ExampleClient
{    
    public class Example
    {
        public void Run()
        {
            IExampleWithFiltering server=ClientFactory.Create<IExampleWithFiltering>("http://yourserver/api"); `
        
            //Per example, you can ask the server to not serialize the IsProgrammer property using levels, limiting the Level to 0.
             List<Person> persons=server.FilterWithLevel(0).GetPersons();
            //Or remove the Attributes property using the ExcludeTags.            
            List<Person> persons=server.FilterWithTags(new string[] { "attr"}).GetPersons();            
            
        }
    }
}

```


following interface to your project

````csharp


TODO

## History

**1.4.3-Alpha**: First Release

## Built With

* [JSON.Net](newtonsoft.com/json/) 
* [RestSharp](http://restsharp.org/)
* [impromptu-interface](https://github.com/ekonbenefits/impromptu-interface)

## Credits

* **Maximo Piva** -[maxpiva](https://github.com/maxpiva)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details


