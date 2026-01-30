# LogicBuilder.Workflow.ComponentModel.Serialization

## Serialization Management

The library implements the core methods for serialization management like:

- `CreateInstance() `: Object creation
- `GetInstance()`:  Object retrieval
- `GetName() / SetName()`: Object naming
- `GetService()`: Service provider functionality
- `AddSerializationProvider() / RemoveSerializationProvider()` Provider management

The WorkflowMarkupSerializationManager in LogicBuilder.Workflow.ComponentModel.Serialization itself implements IDesignerSerializationManager and leverages the core implementations from DesignerSerializationManager using the Decorator pattern.

## Contributing

Contributions are welcome.