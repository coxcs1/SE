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

# Boostrap Panel Table Helper #

```
#!c#
public ActionResult Index()
{
	//create a url helper for creating hyperlinks
	UrlHelper helper = new UrlHelper(this.ControllerContext.RequestContext);
	
	PanelTable table = new PanelTable();//initialize the table
	table.
		setTitle("Test Table").//set the title
		setItemsPerRow(4).//set the number of items to display per row
		setData(new Dictionary<string, string>()//the data that will be displayed in the table
		{
			{ "First Name", "Dana" },
			{ "Middle Name", "Jarred" },
			{ "Last Name", "Light" },
			{ "", "" }
		}).
		setTableButtons(new Dictionary<string, string>()//the buttons that will be displayed in the table
		{
			{ helper.Action("Login", "Account", null), "Go to login" }
		});
	ViewBag.Table = table;//pass table to view
	return View();
}
```