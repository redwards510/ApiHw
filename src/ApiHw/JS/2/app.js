/*
Each fee has a set of funds associated with it. The money associated with each fee is split among the funds based 
on the amount specified in the distribution. Any extra money associated with the order that isn't allocated to a 
fund should be assigned to a generic "Other" fund.

Challenge: Write Javascript that outputs the fund distributions for each order in the orders.json file, and then 
the totals in each fund after processing all orders.

The output should be of the form:

Order id: <order number>  
  Fund: <fund_n>: $<amount>  
  ..  

Order id: <order number>

Total distributions:
  Fund: <fund_n>: $<amount>  
  ..  
  ..  
*/

// NOTE: there is a type of 'flat' in distrubtions list for Birth CErtificate but not Real Property Recording. ignoring until
// more info is gathered. 
var jf = require('jsonfile');

// read files
var fees = jf.readFileSync('./fees.json');
var orders = jf.readFileSync('./orders.json');

var dict = [];
for(var i = 0; i < orders.length; i++) {
    // get easy handle for top level order object
    var o = orders[Object.keys(orders)[i]];
    console.log('Order id: ' + o.order_number);    
    var itemTypes = [];
    var orderTotal = parseFloat(0);
    var orderTotalDistributions = parseFloat(0);
    // loop over each order item in order    
    for (var c = 0; c < o.order_items.length; c++)
    {        
        // start adding to the fee total so we can calculate Other fees later. 
        orderTotal += parseFloat(lookupFee(o.order_items[c].type, o.order_items[c].pages));
        // if we've never seen the item before... 
        if(itemTypes.indexOf(o.order_items[c].type) === -1)
        {
            // add the type to the seen array
            itemTypes.push(o.order_items[c].type);
            // loop over all order items and get count of each unique type of orderitem, and get total pages 
            var numberOfThem = 0;
            var totalPages = 0;
            for (var u = 0; u < o.order_items.length; u++)
            {
                if(o.order_items[u].type === o.order_items[c].type)
                {
                    numberOfThem++;
                    totalPages += parseInt(o.order_items[u].pages);
                }
            }
            // i don't like using this method for two things like this..  refactor.
            orderTotalDistributions += printItemTypeDistribution(o.order_items[c].type, numberOfThem, totalPages)            
        }     
    } // end individual order item processing loop

    if(orderTotal > orderTotalDistributions) {
        var other = orderTotal - orderTotalDistributions;
        addToTotalDistributions("Other", other);
        console.log('    Fund: Other: $' + other); 
    }            
    console.log (' ');
}

console.log('Total distributions:');
for (var k = 0; k < dict.length; k++)
{
    console.log('    Fund: ' + dict[k].type + ': $' + dict[k].total);
}


/// prints the distribution totals for each unique item type and returns the total amount of distributions
function printItemTypeDistribution(itemType, numberOfThem, totalPages)
{        
    var totalDistributions = 0;
    for (var i = 0; i < fees.length; i++)
    {
        var f = fees[Object.keys(fees)[i]];
        if (f.order_item_type === itemType)
        {            
            for(var j = 0; j < f.distributions.length; j++)
            {
                var fundTotal = parseFloat(f.distributions[j].amount) * parseInt(numberOfThem);
                totalDistributions += fundTotal;
                // add to our global dictionary array for later
                addToTotalDistributions(f.distributions[j].name, fundTotal);
                console.log('    Fund: ' + f.distributions[j].name + ': $' + fundTotal);
            }
        }
    }

    return totalDistributions;

}

function addToTotalDistributions(name, total)
{    
    for (var i = 0; i < dict.length; i++)
    {
        if(dict[i].type === name) {
            dict[i].total += parseFloat(total);
            return;
        }
    }
    dict.push({type: name, total: total});
}

function lookupFee(itemType, pages)
{
    var totalFee = 0;    
    for (var i = 0; i < fees.length; i++)
    {
        var f = fees[Object.keys(fees)[i]];
        if (f.order_item_type === itemType)
        {            
            for(var j = 0; j < f.fees.length; j++)
            {
                if (f.fees[j].type === "flat")
                {
                    totalFee += parseFloat(f.fees[j].amount);
                }
                else if(f.fees[j].type === "per-page" && pages > 1)
                {
                    totalFee += parseFloat(f.fees[j].amount) * parseInt(pages - 1);
                }
            }
        }
    }
    return totalFee;
}