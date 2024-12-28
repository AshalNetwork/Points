document.addEventListener("DOMContentLoaded", function () {

    /****************** Showing Password ******************/

    const togglePasswordIcons = document.querySelectorAll('.togglePassword');
    const passwordFields = document.querySelectorAll('.password');

    togglePasswordIcons.forEach((icon, index) => {
        icon.addEventListener('click', function () {
            // Toggle the type attribute
            const passwordField = passwordFields[index]; // Get the corresponding password field
            const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordField.setAttribute('type', type);

            // Toggle the eye icon
            this.classList.toggle('bi-eye-fill');
            this.classList.toggle('bi-eye-slash-fill');
        });
    });


});


/*******************************Accept Sales Request******************************************** */

function AcceptSalesRequest(requestId) {
    // Show confirmation dialog
    $.confirm({
        title: 'confirm Request',
        content: 'Are you sure you want Accept This Request?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-success',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Sales/Accept?Id=' + requestId,
                        type: 'post',
                        success: function (response) {

                            refreshTable('/Sales/Index'); // Optionally, reload the page
                            toastr.success('The Request Has Accepted successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while Accept The Request', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
}; 
/*******************************Reject Sales Request******************************************** */
function RejectSalesRequest(requestId) {
    // Show confirmation dialog
    $.confirm({
        title: 'confirm Request',
        content: 'Are you sure you want Reject This Request?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-danger',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Sales/Reject?Id=' + requestId,
                        type: 'post',
                        success: function (response) {

                            refreshTable('/Sales/Index'); // Optionally, reload the page
                            toastr.success('The Request Has Rejected successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while Reject The Request', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
};
/*******************************Delete User******************************************** */
function deleteUser(userId) {
    // Show confirmation dialog
    $.confirm({
        title: 'Confirm Delete',
        content: 'Are you sure you want to delete?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-danger',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Account/DeleteAccount?UserId=' + userId,
                        type: 'post',
                        success: function (response) {

                            refreshTable('/Users/Index'); // Optionally, reload the page
                            toastr.success('Account deleted successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while deleting the account.', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
};
/*******************************Delete Penality******************************************** */
function deletePenality(penalityId) {
    // Show confirmation dialog
    $.confirm({
        title: 'Confirm Delete',
        content: 'Are you sure you want to delete?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-danger',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Penalities/Delete?Id=' + penalityId,
                        type: 'post',
                        success: function (response) {

                            refreshTable('/Penalities/Index'); // Optionally, reload the page
                            toastr.success('Penality deleted successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while deleting the Task.', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
};
/*******************************Delete Task******************************************** */
function deleteTask(taskId) {
    // Show confirmation dialog
    $.confirm({
        title: 'Confirm Delete',
        content: 'Are you sure you want to delete?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-danger',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Tasks/Delete?Id=' + taskId,
                        type: 'post',
                        success: function (response) {

                            refreshTable('/Tasks/Index'); // Optionally, reload the page
                            toastr.success('Task deleted successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while deleting the Task.', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
};
/*******************************Complete Task******************************************** */
function CompleteTask(taskId) {
    $.ajax({
        url: '/Tasks/CompleteTask?TaskId=' + taskId,
        type: 'PUT',
        success: function () {
            refreshTable('/Tasks/Index');
            toastr.success('Task has been marked as Completed.', 'Success');
        },
        error: function () {
            toastr.error('Failed to update task. Please try again.', 'Error');
        }
    });
}
/*******************************Shift Task******************************************** */
function ShiftTask(taskId) {
    $.ajax({
        url: '/Tasks/Shift?TaskId=' + taskId,
        type: 'PUT',
        success: function () {
            refreshTable('/Tasks/Index');
            toastr.success('Task has been Shifted To Tomorrow', 'Success');
        },
        error: function () {
            toastr.error('Failed to update task. Please try again.', 'Error');
        }
    });
}
/*******************************MAde The Task Under Review ******************************************** */
function UnderReview(taskId,userId) {
    $.ajax({
        url: '/Tasks/UnderReview?TaskId=' + taskId,
        type: 'PUT',
        success: function () {
            refreshTable('/Tasks/UserTasks',userId);
            toastr.success('Task has been marked as Under Review.', 'Success');
        },
        error: function () {
            toastr.error('Failed to update task. Please try again.', 'Error');
        }
    });
}
/*******************************MAde The Task InProgress ******************************************** */
function InProgressTask(taskId,userId) {
    $.ajax({
        url: '/Tasks/InProgress?TaskId=' + taskId,
        type: 'PUT',
        success: function () {
            refreshTable('/Tasks/UserTasks', userId);
            toastr.success('Task has been marked as InProgress.', 'Success');
        },
        error: function () {
            toastr.error('Failed to update task. Please try again.', 'Error');
        }
    });
}
/*******************************End Project******************************************** */

function EndProject(projectId) {
    // Show confirmation dialog
    $.confirm({
        title: 'confirm Request',
        content: 'Are you sure you want Ending This Project?',
        buttons: {
            confirm: {
                text: 'Confirm',
                btnClass: 'btn-success',
                action: function () {
                    // AJAX request
                    $.ajax({
                        url: '/Projects/MarkAsEnded?Id=' + projectId,
                        type: 'put',
                        success: function (response) {                            
                            toastr.success('The Project Has Ended successfully!', 'Success', { timeOut: 5000 })

                        },
                        error: function (xhr, status, error) {
                            console.log("Error:", error);  // Log the error for debugging
                            toastr.error('An error occurred while Ending The Project', 'Error!')
                        }
                    });
                }
            },
            cancel: function () {

            }
        }
    });
}; 
/*******************************Refresh The Table******************************************** */
function refreshTable(url,UserId) {
    $.ajax({
        url: UserId == null ? url : url + "?UserId=" + UserId,
        type: 'GET',
        success: function (result) {
            $('table tbody').html($(result).find('table tbody').html());
            $('.row').html($(result).find('.row').html());
        },
        error: function (xhr, status, error) {
            console.error("Failed to refresh the table: ", error);
            alert("An error occurred while refreshing the table.");
        }
    });
}
