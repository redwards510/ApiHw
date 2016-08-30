// look up how to read json locally
// you can do require on them, but it is sync and they will be static from then on
// you can use fs, but there's a bunch of boilerplate code required. 
// jsonfile eliminates that and makes it cleaner to read, but also introduces a dependency.
// prefer clean code for this solution.

// fight with async style. forgot how that all works in js. requires a total paradigm change in 
// how you approach solutions and i could not figure out how i wanted to: read file1, read file2, process
// answer lies in having callback method run only after files are finished reading, but how do you 
// make sure that BOTH files are finished before calling process() callback 

// orders file is not correct format for json. were this not homework we'd add in fun things
// like error handling for that, but i want to move on. fix problem by cutting out objects from orders.json
// until i find the missing commas. now that they are loading properly, proceed.

// for-in loops would not work, and i read they are not performant in node
// .each from jquery woudlnt work. you don't even have access to jquery in node
// Object.keys the best way to get a handle on our group of top level objects. 

// now we just loop a bunch. Math in javascript is as horrible as I remember. I'm sure there are better libraries for
// handling things as important as money. 


var jf = require('jsonfile');

// read files
var fees = jf.readFileSync('./fees.json');
var orders = jf.readFileSync('./orders.json');


for(var i = 0; i < orders.length; i++) {
    // get easy handle for top level order object
    var o = orders[Object.keys(orders)[i]];
    console.log('Order id: ' + o.order_number);
    var orderTotal = parseFloat(0);
    // loop over each order item in order
    for (var c = 0; c < o.order_items.length; c++)
    {
        // lookup price for order_item
        var price = lookupFee(o.order_items[c].type, o.order_items[c].pages);
        orderTotal += parseFloat(price);
        console.log('    Order item <' + c + '>: $' + price);         
    }
    console.log ('    Order total: $' + orderTotal);
    console.log (' ');
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

/* Desired Output
Order id: <order number>  
   Order item <n>: $<price>  
   ..
   ..

   Order total: <total>
*/
