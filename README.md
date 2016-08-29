# Data Table Helper #


```
#!c#
//create Cereal DataTableModel
var data = new List<object>();
foreach (var item in cereal)
{
	data.Add(new {
		id = item.Id,
		name = item.Name,
		price = item.Price.ToString("C2"),
		amountInStock = item.AmountInStock
	});
}

var cerealDataTableModel = new DataTableModel();
cerealDataTableModel.
	setTitle("Cereal").
	setData(data).
	setSearchSort(true).
	setHeaders(new List<object>()
	{
		new {
			Name = "Name",
			Field = "name"
		},
		new {
			Name = "Price",
			Field = "price"
		},
		new {
			Name = "Amount In Stock",
			Field = "amountInStock"
		},
	}).
	setActions(new List<object>()
	{
		new {
			text = "Buy",
			url = "/shoppingcart/buy/{{id}}"
		}
	});
```