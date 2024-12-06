$(document).ready(function () {
    var table = $('#psychologistTable').DataTable({
        ajax: {
            url: '/Appointment/GetAll',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            { "data": "username" },
            { "data": "email" },
            {
                data: null,
                title: 'Randevu Al',
                render: function (data, type, row) {
                    return `<button class="btn btn-primary book-appointment" data-id="${row.id}" data-bs-toggle="modal" data-bs-target="#appointmentModal">Randevu Al</button>`;
                }
            }
        ]
    });

    $('#psychologistTable').on('click', '.book-appointment', function () {
        var psychologistId = $(this).data('id');
        $('#psychologistId').val(psychologistId);
        $('#appointmentDate').val(''); // Tarih alanını temizle
        $('#timeSlotsContainer').html(''); // Saat dilimlerini temizle

    });

    $('#appointmentDate').on('change', function () {
        var psychologistId = $('#psychologistId').val();
        var selectedDate = $(this).val();

        console.log('Selected Date:', selectedDate);

        if (!psychologistId || !selectedDate) {
            alert('Lütfen bir tarih seçiniz.');
            return;
        }

        $.ajax({
            url: '/Appointment/GetAvailableTimeSlots',
            type: 'GET',
            data: {
                psychologistId: psychologistId,
                selectedDate: selectedDate
            },
            success: function (response) {
                var timeSlotsHtml = '';
                response.forEach(function (timeSlot) {
                    timeSlotsHtml += `<button type="button" class="btn btn-secondary time-slot" data-time="${timeSlot}">${timeSlot}</button>`;
                });
                $('#timeSlotsContainer').html(timeSlotsHtml);
            },
            error: function (xhr) {
                alert('Saat dilimleri alınırken hata oluştu.');
                console.error(xhr);
            }
        });
    });

    $('#appointmentForm').on('submit', function (e) {
        e.preventDefault();

        var psychologistId = $('#psychologistId').val();
        var selectedDate = $('#appointmentDate').val();
        var selectedTime = $('.time-slot.active').data('time');

        console.log('Form submitted');
        console.log('Psychologist ID:', psychologistId);
        console.log('Selected Date:', selectedDate);
        console.log('Selected Time:', selectedTime);

        if (!psychologistId || !selectedDate || !selectedTime) {
            alert('Lütfen tüm alanları doldurunuz.');
            return;
        }

        $.ajax({
            url: '/Appointment/BookAppointment',
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded',
            data: {
                psychologistId: psychologistId,
                selectedDate: selectedDate,
                selectedTime: selectedTime
            },
            success: function (response) {
                alert(response);
                $('#appointmentModal').modal('hide');
            },
            error: function (xhr) {
                alert('Seçilen zaman dilimi uygun değil veya dolmuş.');
                console.error(xhr);
            }
        });
    });

    $('#timeSlotsContainer').on('click', '.time-slot', function () {
        $('.time-slot').removeClass('active');
        $(this).addClass('active');
    });
});


$(document).ready(function () {
    $('#appointmentTable').DataTable({
        ajax: {
            url: '@Url.Action("GetAllMyAppointments", "Appointment")',
            type: 'GET',
            dataSrc: 'data'
        },
        columns: [
            {
                "data": "appointmentDateTime", render: function (data) {
                    return new Date(data).toLocaleDateString('tr-TR');
                }
            },
            {
                "data": "appointmentStartTime", render: function (data) {
                    return data.substring(0, 5); // Sadece saati ve dakikayı göster
                }
            },
            { "data": "userName" },
            {
                "data": "status", render: function (data) {
                    return data; // Enum adını doğrudan döndür
                }
            }
        ]
    });
});