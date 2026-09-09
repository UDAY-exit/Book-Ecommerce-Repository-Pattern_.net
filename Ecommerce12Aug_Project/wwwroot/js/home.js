$(document).ready(function () {

    var table = $('#productTable').DataTable({
        paging: true,
        pageLength: 8,
        searching: true,
        ordering: false,
       
        lengthChange: false,
        dom: 'lrtip'
    });

    function search() {

        var value = $('#customSearchBox').val();
        var column = $('#searchColumn').val();

        table.search('');
        table.columns().search('');

        if (column == 'all') {
            table.search(value);
        }
        else {
            table.column(column).search(value);
        }

        table.draw();
    }

    $('#searchBtn').click(search);

    $('#customSearchBox').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            search();
        }
    });

    $('#searchColumn').change(search);

});