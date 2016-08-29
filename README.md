#### Data Table Model Helper

To setup apache, setup a virtual host to point to the public/ directory of the
project. It should look something like below:
The httpd.conf file is located in `C:\wamp\bin\apache\apache2.4.9\conf\httpd.conf`
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