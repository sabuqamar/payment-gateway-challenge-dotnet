## Directory tree structure
```
├──PaymentGateway.Api
│       ├── Controllers
│       ├── Enums
│       ├── Exceptions
│       ├── Models
│       │   ├── Requests
│       │   ├── Responses
│       │   └── Validation
│       └──Services
│  
└── PaymentGateway.Api.Tests
        ├── IntegrationTests
        └── UnitTests
```

The above tree explains the high level structure of the main directories in this project. It follows the controller-service-repository design pattern.
The main flow of data goes as follows: user calls API endpoint in the controller layer, which then calls the services layer. The services consist of a bank service which calls the bank simulator, and payment repository which handles data storage of payements.
The model directory contains request and response classes for the API Gateway and the bank simulator. It also contains attribute validation classes which validate the attributes of the API Gateway request body.

The tests are split into two directories: unit and integration tests. Integration tests spin up the program and tests the flow of data through all layers. The unit tests are specific for the bank service in isolation and the payment repository in isolation. The test cases try to cover all the scenarios in which a funtion would be successful as well as throw an error, such as unaccepted request value format. The names of the tests should be self-explanatory.
