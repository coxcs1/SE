# Data Table Model Helper #

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
