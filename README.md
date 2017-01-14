# Nancy.Rest.Client

Dynamic proxy client generation for Nancy.

## Prerequisites

A server using [Nancy](http://nancyfx.org) & [Nancy.Rest.Module](https://github.com/maxpiva/Nancy.Rest.Module).

You should read [Nancy.Rest.Module](https://github.com/maxpiva/Nancy.Rest.Module) documentation to understand how it works before continuing reading.

## Installation

Add [Nancy.Rest.Client](https://github.com/maxpiva/Nancy.Rest.Client) and [Nancy.Rest.Annotations](https://github.com/maxpiva/NNancy.Rest.Annotations) to your client project.

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

[Nancy.Rest.Client](https://github.com/maxpiva/Nancy.Rest.Client) includes this interface.

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
        
            //Per example, you can ask the server to not serialize any property with level bigger than the number provided.
            //Here we can filter the IsProgrammer property using levels.
            List<Person> persons=server.FilterWithLevel(0).GetPersons();
            //Or remove the Attributes property using Tags.            
            List<Person> persons=server.FilterWithTags(new string[] { "Attr"}).GetPersons();            
            
        }
    }
}

```

###Controlable deserialization

Imagine you have your poco models from the server, but you need to add some properties, methods or INotifyPropertyChanged to that objects, you create a child class from the model, and add all those things. The problem is, the deserialzer will deserialize your poco model, so you have to create a new child class, and copy all properties to your child. Nancy.Rest.Client have the capability of deserializing the objects to child objects.

####Client model

```csharp

namespace Nancy.Rest.ExampleClient
{    
    public class ClientPerson : Person
    {
        public bool IsSuperman { get; set; }
        
        public void DoSomeNastyThings()
        {
        //Kidding
        }
    }
}

```
####Example

```csharp

namespace Nancy.Rest.ExampleClient
{    
    public class Example
    {
        public void Run()
        {
            Dictionary<Type,Type> mappings=new Dictionary<Type,Type>();
            //Here we add the mapping, we want every person to be deserialized as ClientPerson
            mappings.Add(typeof(Person), typeof(ClientPerson));
            IExample server=ClientFactory.Create<IExample>("http://yourserver/api", mappings); `
            //Here is your list of client persons
            List<ClientPerson> persons=server.GetPersons().Cast<ClientPerson>.ToList();
        }
    }
}

```

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


