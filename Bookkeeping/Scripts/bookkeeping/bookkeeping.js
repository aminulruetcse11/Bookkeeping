$(document).ready(function () {
    $(document).on('focus', 'input[type="number"]', function () {
        $(this).select();
    });
});
function populateData(year) {
    $.ajax({
        type: "GET",
        url: "/Home/GetIncomeCost?Year=" + year,
        contentType: "application/json",
        success: function (data) {
            //console.log(data);
            PopulateIncomeCostTable(data.inex);
            PopulateReconciliationTable(data.reconIncome, "Income");
            PopulateReconciliationTable(data.reconExpense, "Expense");
            CalculateFinalAndCumulativeResult();
        },
        error: function (err) {
            console.error(err);
        }
    })
}

function PopulateIncomeCostTable(data) {
    $("#exin_table>tbody").empty();
    var newRow = '';
    var cols = '';
    for (var i = 0; i < 5; i++) {
        newRow = $('<tr class="exin">');
        cols = "";
        cols += '<td></td>';
        if (i === 0) {
            cols += '<td>Income</td>';
        }
        else if (i === 1) {
            cols += '<td>Cumulative Income</td>';
        }
        else if (i === 2) {
            cols += '<td>Cost </td>';
        }
        else if (i === 3) {
            cols += '<td>Cumulative Cost</td>';
        }
        else if (i === 4) {
            cols += '<td><b>Result</b></td>';
        }

        $.each(data, function (index, item) {
            if (i === 0) {
                cols += '<td>' + item.Income + '</td>';
            }
            else if (i === 1) {
                cols += '<td>' + item.CumulativeIncome + '</td>';
            }
            else if (i === 2) {
                cols += '<td>' + item.Cost + '</td>';
            }
            else if (i === 3) {
                cols += '<td>' + item.CumulativeCost + '</td>';
            }
            else if (i === 4) {
                cols += '<td id="exinResult_' + index + '"><input style="width:80px !important" type="number" value="' + item.Result + '" readonly></b></td>';
            }
        });

        newRow.append(cols);

        $("#exin_table").append(newRow);
    }

    newRow = $("<tr>");
    cols = '<td></td>';
    cols += '<td colspan="13" class="text-center"><b>Reconciliation</b></td>';
    newRow.append(cols);
    $("#exin_table").append(newRow);

}


function PopulateReconciliationTable(data, Type) {
    var newRow = '';
    var cols = '';
    var counter = 1;

    $.each(data, function (i, item) {
        newRow = $('<tr class="Rec_' + Type + '">');
        cols = "";
        if (Type == "Expense") {
            if (data.length !== i + 1) {
                cols += '<td>' + Type + '</td>';
            }
            else
                cols += '<td></td>';
        }
        else
            cols += '<td>' + Type + '</td>';



        cols += '<td><input value="' + item.HeadID + '" name="' + Type + '_HeadID_' + i + '"  type="hidden"/>' + item.HeadName + '</td>';
        if (data.length === i + 1 && Type === "Expense") {
            $.each(item.ReconInEXList, function (index, sitem) {
                cols += '<td id="Recresult_' + index + '"><input readonly style="width:80px !important" type="number" name="' + counter + '" value="' + sitem.Income + '"/></td>';
                counter++;
            });
        }
        else {
            $.each(item.ReconInEXList, function (index, sitem) {
                cols += '<td class=' + index + '><input style="width:80px !important" type="number" name="' + Type + '_' + counter + '" value="' + sitem.Income + '"/></td>';
                counter++;
            });
        }


        newRow.append(cols);

        $("#exin_table").append(newRow);
    })



}

//Reconciliation input change event
$(document).on('input', 'input[type="number"]', function () {
    console.log($(this).val());

    var inputParent_td = $(this).parent("td");
    var colNumber = parseInt(inputParent_td[0].className);
    var incomeInputs = $('td.' + colNumber).find('input[name*="Income_"]');
    var totalIncome = 0;
    var totalCost = 0;
    $.each(incomeInputs, function (i, item) {
        totalIncome += item.value === "" ? 0.00 : parseFloat(item.value);
    });

    var expenseInputs = $('td.' + colNumber).find('input[name*="Expense_"]');
    $.each(expenseInputs, function (i, item) {
        totalCost += item.value === "" ? 0.00 : parseFloat(item.value);
    });

    var result = totalIncome - totalCost;
    var resultanttd = $('td#Recresult_' + colNumber).find('input[type="number"]');
    resultanttd.val(result);
    CalculateFinalAndCumulativeResult();
});


function CalculateFinalAndCumulativeResult() {
    $("tr#FinalResult").remove();
    $("tr#CumulativeResult").remove();

    var newRow = '';
    var cols = '';
    newRow = $('<tr id="FinalResult">');

    var CnewRow = '';
    var Ccols = '';
    CnewRow = $('<tr id="CumulativeResult">');

    var exinResult = $('[id*="exinResult_"]').find('input[type="number"]');
    var recResult = $('[id*="Recresult_"]').find('input[type="number"]');
    var finalResult = 0;
    var RecCumuResult = 0;
    cols += '<td></td>';
    cols += '<td>Final result</td>';

    Ccols += '<td></td>';
    Ccols += '<td>Cumulative result</td>';

    for (var i = 0; i < 12; i++) {
        finalResult = parseFloat(exinResult[i].value) + parseFloat(recResult[i].value);
        RecCumuResult += finalResult;
        cols += '<td>' + finalResult + '</td>';
        Ccols += '<td>' + RecCumuResult + '</td>';
    }

    newRow.append(cols);
    CnewRow.append(Ccols);


    $("#exin_table").append(newRow);
    $("#exin_table").append(CnewRow);

}