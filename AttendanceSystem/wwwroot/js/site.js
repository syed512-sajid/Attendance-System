var Attandance = Attandance || {};

Attandance.Login = {

    // =========================================================
    // Loader
    // =========================================================

    ShowLoader: function () {

        var loader = document.getElementById('globalLoader');

        if (loader) {
            loader.classList.add('active');
        }

    },


    HideLoader: function () {

        var loader = document.getElementById('globalLoader');

        if (loader) {
            loader.classList.remove('active');
        }

    },


    // =========================================================
    // Moving Watermark
    // =========================================================

    Watermark: {
        Element: null,
        X: 40,
        Y: 40,
        SpeedX: 1.8,
        SpeedY: 1.4
    },


    InitWatermark: function () {

        Attandance.Login.Watermark.Element =
            document.querySelector('.floating-watermark');

        if (!Attandance.Login.Watermark.Element) {
            return;
        }

        Attandance.Login.MoveWatermark();

    },


    MoveWatermark: function () {

        var watermark =
            Attandance.Login.Watermark.Element;

        if (!watermark) {
            return;
        }

        var data =
            Attandance.Login.Watermark;


        // Screen boundaries
        var maxX =
            window.innerWidth - watermark.offsetWidth;

        var maxY =
            window.innerHeight - watermark.offsetHeight;


        maxX = Math.max(0, maxX);
        maxY = Math.max(0, maxY);


        // Move watermark
        data.X += data.SpeedX;
        data.Y += data.SpeedY;


        // =====================================================
        // Right Border
        // =====================================================

        if (data.X >= maxX) {

            data.X = maxX;

            data.SpeedX =
                -Math.abs(data.SpeedX);

        }


        // =====================================================
        // Left Border
        // =====================================================

        if (data.X <= 0) {

            data.X = 0;

            data.SpeedX =
                Math.abs(data.SpeedX);

        }


        // =====================================================
        // Bottom Border
        // =====================================================

        if (data.Y >= maxY) {

            data.Y = maxY;

            data.SpeedY =
                -Math.abs(data.SpeedY);

        }


        // =====================================================
        // Top Border
        // =====================================================

        if (data.Y <= 0) {

            data.Y = 0;

            data.SpeedY =
                Math.abs(data.SpeedY);

        }


        // Apply position
        watermark.style.transform =
            'translate3d(' +
            data.X +
            'px, ' +
            data.Y +
            'px, 0)';


        // Continue movement
        requestAnimationFrame(
            Attandance.Login.MoveWatermark
        );

    },


    // =========================================================
    // Window Resize
    // =========================================================

    InitResize: function () {

        window.addEventListener('resize', function () {

            var watermark =
                Attandance.Login.Watermark.Element;

            if (!watermark) {
                return;
            }


            var maxX =
                window.innerWidth -
                watermark.offsetWidth;

            var maxY =
                window.innerHeight -
                watermark.offsetHeight;


            maxX = Math.max(0, maxX);
            maxY = Math.max(0, maxY);


            Attandance.Login.Watermark.X =
                Math.min(
                    Math.max(
                        Attandance.Login.Watermark.X,
                        0
                    ),
                    maxX
                );


            Attandance.Login.Watermark.Y =
                Math.min(
                    Math.max(
                        Attandance.Login.Watermark.Y,
                        0
                    ),
                    maxY
                );

        });

    },


    // =========================================================
    // Form Submit
    // =========================================================

    InitForms: function () {

        document
            .querySelectorAll('form')
            .forEach(function (form) {

                form.addEventListener('submit', function () {

                    Attandance.Login.ShowLoader();

                });

            });

    },


    // =========================================================
    // Internal Navigation
    // =========================================================

    InitLinks: function () {

        document
            .querySelectorAll('a[href]')
            .forEach(function (link) {

                var href =
                    link.getAttribute('href');

                if (!href) return;

                if (href.startsWith('#')) return;

                if (href.startsWith('mailto:')) return;

                if (href.startsWith('tel:')) return;

                if (href.startsWith('javascript:')) return;

                if (link.target === '_blank') return;

                if (link.hasAttribute('download')) return;


                try {

                    var url =
                        new URL(
                            href,
                            window.location.href
                        );

                    // External link
                    if (
                        url.origin !==
                        window.location.origin
                    ) {
                        return;
                    }

                }
                catch (e) {

                    // Relative URL
                    // Continue

                }


                link.addEventListener(
                    'click',
                    function () {

                        Attandance.Login.ShowLoader();

                    }
                );

            });

    },

    InitPasswordToggle: function () {

        var password = document.getElementById('password');
        var toggle = document.getElementById('togglePassword');
        var icon = document.getElementById('passwordIcon');

        if (!password || !toggle || !icon) {
            return;
        }

        toggle.addEventListener('click', function () {

            if (password.type === 'password') {

                password.type = 'text';

                icon.classList.remove('bi-eye');
                icon.classList.add('bi-eye-slash');

                toggle.setAttribute(
                    'aria-label',
                    'Hide password'
                );

            } else {

                password.type = 'password';

                icon.classList.remove('bi-eye-slash');
                icon.classList.add('bi-eye');

                toggle.setAttribute(
                    'aria-label',
                    'Show password'
                );
            }
        });
    },
    // =========================================================
    // Init View
    // =========================================================

    InitView: function () {

        Attandance.Login.InitWatermark();

        Attandance.Login.InitResize();

        Attandance.Login.InitForms();

        Attandance.Login.InitLinks();

        Attandance.Login.InitPasswordToggle();

    }

};
// =============================================================
// Assign Leave (Admin) - Dynamic Field Toggle
// =============================================================

Attandance.AssignLeave = {

    TypeEl: null,
    ToDateGroup: null,
    ToDateInput: null,
    FromDateInput: null,
    SessionGroup: null,
    HoursGroup: null,
    FromTimeGroup: null,
    ToTimeGroup: null,
    FormEl: null,


    UpdateFields: function () {

        var type = Attandance.AssignLeave.TypeEl.value;

        // Half Day
        Attandance.AssignLeave.SessionGroup.classList.toggle(
            'd-none',
            type !== 'Half Day'
        );

        // Short Leave - Hours
        Attandance.AssignLeave.HoursGroup.classList.toggle(
            'd-none',
            type !== 'Short Leave'
        );

        // Short Leave - From Time
        Attandance.AssignLeave.FromTimeGroup.classList.toggle(
            'd-none',
            type !== 'Short Leave'
        );

        // Short Leave - To Time
        Attandance.AssignLeave.ToTimeGroup.classList.toggle(
            'd-none',
            type !== 'Short Leave'
        );


        // Half Day / Short Leave = single date
        if (type === 'Half Day' || type === 'Short Leave') {

            Attandance.AssignLeave.ToDateGroup.classList.add('d-none');

            Attandance.AssignLeave.ToDateInput.required = false;

        } else {

            Attandance.AssignLeave.ToDateGroup.classList.remove('d-none');

            Attandance.AssignLeave.ToDateInput.required = true;
        }

    },


    SyncToDate: function () {

        var type = Attandance.AssignLeave.TypeEl.value;

        if (type === 'Half Day' || type === 'Short Leave') {

            Attandance.AssignLeave.ToDateInput.value =
                Attandance.AssignLeave.FromDateInput.value;

        }

    },


    InitView: function () {

        Attandance.AssignLeave.TypeEl =
            document.getElementById('aLeaveType');

        Attandance.AssignLeave.ToDateGroup =
            document.getElementById('aToDateGroup');

        Attandance.AssignLeave.ToDateInput =
            document.getElementById('aToDate');

        Attandance.AssignLeave.FromDateInput =
            document.getElementById('aFromDate');

        Attandance.AssignLeave.SessionGroup =
            document.getElementById('aSessionGroup');

        Attandance.AssignLeave.HoursGroup =
            document.getElementById('aHoursGroup');

        Attandance.AssignLeave.FromTimeGroup =
            document.getElementById('aFromTimeGroup');

        Attandance.AssignLeave.ToTimeGroup =
            document.getElementById('aToTimeGroup');

        Attandance.AssignLeave.FormEl =
            document.getElementById('assignLeaveForm');


        if (!Attandance.AssignLeave.TypeEl ||
            !Attandance.AssignLeave.FormEl) {
            return;
        }


        Attandance.AssignLeave.TypeEl.addEventListener(
            'change',
            Attandance.AssignLeave.UpdateFields
        );


        Attandance.AssignLeave.FromDateInput.addEventListener(
            'change',
            Attandance.AssignLeave.SyncToDate
        );


        Attandance.AssignLeave.FormEl.addEventListener(
            'submit',
            Attandance.AssignLeave.SyncToDate
        );


        Attandance.AssignLeave.UpdateFields();

    }

};

// =============================================================
// Employee Management - Add / Edit Employee
// =============================================================

Attandance.Employee = {

    SelectedRow: null,
    FormEl: null,
    CreateUrl: null,
    UpdateUrl: null,
    PasswordUrl: null,

    EmpIdInput: null,
    FullNameInput: null,
    UsernameInput: null,
    RoleInput: null,
    PasswordInput: null,
    PasswordHint: null,

    FormTitle: null,
    SubmitBtn: null,
    CancelWrap: null,
    CancelBtn: null,

    InitView: function () {

        Attandance.Employee.FormEl = document.getElementById('employeeForm');
        if (!Attandance.Employee.FormEl) return;

        Attandance.Employee.CreateUrl = Attandance.Employee.FormEl.dataset.createUrl;
        Attandance.Employee.UpdateUrl = Attandance.Employee.FormEl.dataset.updateUrl;
        Attandance.Employee.PasswordUrl = Attandance.Employee.FormEl.dataset.passwordUrl;

        Attandance.Employee.EmpIdInput = document.getElementById('empId');
        Attandance.Employee.FullNameInput = document.getElementById('empFullName');
        Attandance.Employee.UsernameInput = document.getElementById('empUsername');
        Attandance.Employee.RoleInput = document.getElementById('empRole');
        Attandance.Employee.PasswordInput = document.getElementById('newEmpPassword');
        Attandance.Employee.PasswordHint = document.getElementById('passwordHint');
        Attandance.Employee.FormTitle = document.getElementById('formTitle');
        Attandance.Employee.SubmitBtn = document.getElementById('submitBtn');
        Attandance.Employee.CancelWrap = document.getElementById('cancelWrap');
        Attandance.Employee.CancelBtn = document.getElementById('cancelEditBtn');

        // Sirf Name/Username/Role cell par click kare to edit khule (Action column safe rehta hai)
        document.querySelectorAll('.employee-edit-trigger').forEach(function (cell) {
            cell.addEventListener('click', function () {
                var row = cell.closest('tr');
                Attandance.Employee.Edit(row);
            });
        });

        if (Attandance.Employee.CancelBtn) {
            Attandance.Employee.CancelBtn.addEventListener('click', function () {
                Attandance.Employee.ResetForm();
            });
        }

    },

    Edit: function (row) {

        document.querySelectorAll('.employee-row').forEach(function (r) {
            r.classList.remove('employee-selected');
        });
        row.classList.add('employee-selected');
        Attandance.Employee.SelectedRow = row;

        Attandance.Employee.EmpIdInput.value = row.dataset.id;
        Attandance.Employee.FullNameInput.value = row.dataset.fullname;
        Attandance.Employee.UsernameInput.value = row.dataset.username;
        Attandance.Employee.RoleInput.value = row.dataset.role;

        Attandance.Employee.PasswordInput.value = '';
        Attandance.Employee.PasswordInput.required = false;
        Attandance.Employee.PasswordHint.classList.add('d-none');

        // Current password AJAX se laa kar field mein bhar do
        if (Attandance.Employee.PasswordUrl) {
            fetch(Attandance.Employee.PasswordUrl + '?employeeId=' + row.dataset.id)
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    if (data && data.password) {
                        Attandance.Employee.PasswordInput.value = data.password;
                        Attandance.Employee.PasswordHint.classList.remove('d-none');
                    }
                })
                .catch(function () { /* purana hash-based account - blank hi rehne den */ });
        }

        Attandance.Employee.FormEl.action = Attandance.Employee.UpdateUrl;
        Attandance.Employee.FormTitle.textContent = 'Edit Employee';
        Attandance.Employee.SubmitBtn.textContent = 'Update';
        Attandance.Employee.CancelWrap.style.display = 'block';

        window.scrollTo({ top: 0, behavior: 'smooth' });
    },

    ResetForm: function () {

        Attandance.Employee.FormEl.reset();
        Attandance.Employee.EmpIdInput.value = '0';
        Attandance.Employee.PasswordInput.required = true;
        Attandance.Employee.PasswordHint.classList.add('d-none');
        Attandance.Employee.FormEl.action = Attandance.Employee.CreateUrl;
        Attandance.Employee.FormTitle.textContent = 'Add New Employee';
        Attandance.Employee.SubmitBtn.textContent = 'Add';
        Attandance.Employee.CancelWrap.style.display = 'none';

        document.querySelectorAll('.employee-row').forEach(function (row) {
            row.classList.remove('employee-selected');
        });
        Attandance.Employee.SelectedRow = null;
    }

};

// =============================================================
// Employee list - Search + Active/Inactive filter
// =============================================================

Attandance.EmployeeFilter = {

    SearchInput: null,
    FilterButtons: null,
    CurrentStatus: 'all',

    InitView: function () {

        Attandance.EmployeeFilter.SearchInput = document.getElementById('employeeSearch');
        Attandance.EmployeeFilter.FilterButtons = document.querySelectorAll('[data-filter]');

        if (!Attandance.EmployeeFilter.SearchInput && Attandance.EmployeeFilter.FilterButtons.length === 0) return;

        if (Attandance.EmployeeFilter.SearchInput) {
            Attandance.EmployeeFilter.SearchInput.addEventListener('input', Attandance.EmployeeFilter.Apply);
        }

        Attandance.EmployeeFilter.FilterButtons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                Attandance.EmployeeFilter.FilterButtons.forEach(function (b) { b.classList.remove('active'); });
                btn.classList.add('active');
                Attandance.EmployeeFilter.CurrentStatus = btn.dataset.filter;
                Attandance.EmployeeFilter.Apply();
            });
        });
    },

    Apply: function () {

        var term = (Attandance.EmployeeFilter.SearchInput ? Attandance.EmployeeFilter.SearchInput.value : '').toLowerCase().trim();
        var status = Attandance.EmployeeFilter.CurrentStatus;

        document.querySelectorAll('.employee-row').forEach(function (row) {
            var name = (row.dataset.fullname || '').toLowerCase();
            var username = (row.dataset.username || '').toLowerCase();
            var matchesSearch = !term || name.indexOf(term) !== -1 || username.indexOf(term) !== -1;
            var matchesStatus = status === 'all' || row.dataset.active === status;

            row.style.display = (matchesSearch && matchesStatus) ? '' : 'none';
        });
    }

};
// =============================================================
// Leave Request (Employee) - Dynamic Field Toggle
// =============================================================

Attandance.LeaveRequest = {

    TypeEl: null,
    ToDateGroup: null,
    ToDateInput: null,
    FromDateInput: null,
    SessionGroup: null,
    HoursGroup: null,      // From Time group
    ToTimeGroup: null,     // To Time group
    FormEl: null,


    UpdateFields: function () {

        var type = Attandance.LeaveRequest.TypeEl.value;

        Attandance.LeaveRequest.SessionGroup.classList.toggle(
            'd-none', type !== 'Half Day'
        );

        Attandance.LeaveRequest.HoursGroup.classList.toggle(
            'd-none', type !== 'Short Leave'
        );

        Attandance.LeaveRequest.ToTimeGroup.classList.toggle(
            'd-none', type !== 'Short Leave'
        );

        if (type === 'Half Day' || type === 'Short Leave') {

            Attandance.LeaveRequest.ToDateGroup.classList.add('d-none');
            Attandance.LeaveRequest.ToDateInput.required = false;

        } else {

            Attandance.LeaveRequest.ToDateGroup.classList.remove('d-none');
            Attandance.LeaveRequest.ToDateInput.required = true;

        }

    },


    SyncToDate: function () {

        var type = Attandance.LeaveRequest.TypeEl.value;

        if (type === 'Half Day' || type === 'Short Leave') {

            Attandance.LeaveRequest.ToDateInput.value =
                Attandance.LeaveRequest.FromDateInput.value;

        }

    },


    InitView: function () {

        Attandance.LeaveRequest.TypeEl = document.getElementById('leaveType');
        Attandance.LeaveRequest.ToDateGroup = document.getElementById('toDateGroup');
        Attandance.LeaveRequest.ToDateInput = document.getElementById('toDate');
        Attandance.LeaveRequest.FromDateInput = document.getElementById('fromDate');
        Attandance.LeaveRequest.SessionGroup = document.getElementById('sessionGroup');
        Attandance.LeaveRequest.HoursGroup = document.getElementById('hoursGroup');
        Attandance.LeaveRequest.ToTimeGroup = document.getElementById('toTimeGroup');   // 👈 added
        Attandance.LeaveRequest.FormEl = document.getElementById('leaveRequestForm');

        if (!Attandance.LeaveRequest.TypeEl || !Attandance.LeaveRequest.FormEl) {
            return;
        }

        Attandance.LeaveRequest.TypeEl.addEventListener(
            'change', Attandance.LeaveRequest.UpdateFields
        );

        Attandance.LeaveRequest.FromDateInput.addEventListener(
            'change', Attandance.LeaveRequest.SyncToDate
        );

        Attandance.LeaveRequest.FormEl.addEventListener(
            'submit', Attandance.LeaveRequest.SyncToDate
        );

        Attandance.LeaveRequest.UpdateFields();

    }

};
// =============================================================
// Generic Confirm Modal (replaces native confirm())
// =============================================================

Attandance.Confirm = {

    ModalEl: null,
    Modal: null,
    BodyEl: null,
    OkBtn: null,
    PendingForm: null,


    Init: function () {

        var modalEl = document.getElementById('confirmModal');

        if (!modalEl) {
            return;
        }

        Attandance.Confirm.ModalEl = modalEl;
        Attandance.Confirm.Modal = new bootstrap.Modal(modalEl);
        Attandance.Confirm.BodyEl = document.getElementById('confirmModalBody');
        Attandance.Confirm.OkBtn = document.getElementById('confirmModalOkBtn');

        document.querySelectorAll('form[data-confirm]').forEach(function (form) {

            form.addEventListener('submit', function (e) {

                if (form.dataset.confirmed === 'true') {
                    return; // already confirmed, submit normally
                }

                e.preventDefault();
                e.stopImmediatePropagation();   // 👈 added — Login's loader listener ko chalne se roke

                Attandance.Confirm.PendingForm = form;
                Attandance.Confirm.BodyEl.textContent = form.dataset.confirm;
                Attandance.Confirm.Modal.show();

            });

        });

        Attandance.Confirm.OkBtn.addEventListener('click', function () {

            var form = Attandance.Confirm.PendingForm;

            if (!form) {
                return;
            }

            Attandance.Confirm.Modal.hide();

            form.dataset.confirmed = 'true';

            if (form.requestSubmit) {
                form.requestSubmit();
            } else {
                form.submit();
            }

        });

    }

};
// =============================================================
// Leave Balance (Admin) - Employee dropdown change par auto-load
// =============================================================

Attandance.LeaveBalance = {

    EmpSelect: null,
    TotalInput: null,
    Year: null,
    Url: null,

    LoadBalance: function () {

        if (!Attandance.LeaveBalance.EmpSelect.value) return;

        fetch(
            Attandance.LeaveBalance.Url +
            '?employeeId=' + Attandance.LeaveBalance.EmpSelect.value +
            '&year=' + Attandance.LeaveBalance.Year
        )
            .then(function (res) { return res.json(); })
            .then(function (data) {
                Attandance.LeaveBalance.TotalInput.value = data.totalLeaves;
            });

    },

    InitView: function () {

        Attandance.LeaveBalance.EmpSelect = document.getElementById('lbEmployeeId');
        Attandance.LeaveBalance.TotalInput = document.getElementById('lbTotalLeaves');

        if (!Attandance.LeaveBalance.EmpSelect || !Attandance.LeaveBalance.TotalInput) {
            return;
        }

        var yearEl = document.getElementById('lbYear');
        var urlEl = document.getElementById('lbGetBalanceUrl');

        Attandance.LeaveBalance.Year = yearEl ? yearEl.value : new Date().getFullYear();
        Attandance.LeaveBalance.Url = urlEl ? urlEl.value : '';

        Attandance.LeaveBalance.EmpSelect.addEventListener(
            'change',
            Attandance.LeaveBalance.LoadBalance
        );

        Attandance.LeaveBalance.LoadBalance();

    }

};
// =============================================================
// Forgot Password
// CAPTCHA + Administrator Email
// =============================================================

Attandance.ForgotPassword = {

    CaptchaCode: '',

    CaptchaElement: null,

    CaptchaInput: null,

    RefreshButton: null,

    ShowCaptchaButton: null,

    CaptchaSection: null,

    AdministratorSection: null,

    SuccessMessage: null,

    ErrorMessage: null,

    SendMailButton: null,


    // ---------------------------------------------------------
    // Generate CAPTCHA
    // ---------------------------------------------------------

    GenerateCaptcha: function () {

        var characters =
            'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';


        Attandance.ForgotPassword.CaptchaCode = '';


        for (var i = 0; i < 6; i++) {

            Attandance.ForgotPassword.CaptchaCode +=
                characters.charAt(
                    Math.floor(
                        Math.random() * characters.length
                    )
                );

        }


        Attandance.ForgotPassword.CaptchaElement.textContent =
            Attandance.ForgotPassword.CaptchaCode;


        // Clear input

        Attandance.ForgotPassword.CaptchaInput.value = '';


        // Hide messages

        Attandance.ForgotPassword.SuccessMessage.style.display =
            'none';

        Attandance.ForgotPassword.ErrorMessage.style.display =
            'none';


        // Hide administrator

        Attandance.ForgotPassword.AdministratorSection.style.display =
            'none';

    },


    // ---------------------------------------------------------
    // Show CAPTCHA
    // ---------------------------------------------------------

    ShowCaptcha: function () {

        Attandance.ForgotPassword.CaptchaSection.style.display =
            'block';


        Attandance.ForgotPassword.ShowCaptchaButton.style.display =
            'none';


        Attandance.ForgotPassword.GenerateCaptcha();


        Attandance.ForgotPassword.CaptchaInput.focus();

    },


    // ---------------------------------------------------------
    // Verify CAPTCHA
    // ---------------------------------------------------------

    VerifyCaptcha: function () {

        var entered =
            Attandance.ForgotPassword.CaptchaInput.value
                .trim()
                .toUpperCase();


        // Empty

        if (entered.length === 0) {

            Attandance.ForgotPassword.SuccessMessage.style.display =
                'none';

            Attandance.ForgotPassword.ErrorMessage.style.display =
                'none';

            Attandance.ForgotPassword.AdministratorSection.style.display =
                'none';

            return;
        }


        // Correct CAPTCHA

        if (
            entered ===
            Attandance.ForgotPassword.CaptchaCode
        ) {

            Attandance.ForgotPassword.SuccessMessage.style.display =
                'block';


            Attandance.ForgotPassword.ErrorMessage.style.display =
                'none';


            // Show Administrator Email + Send Mail

            Attandance.ForgotPassword.AdministratorSection.style.display =
                'block';

        }


        // Wrong CAPTCHA

        else {

            Attandance.ForgotPassword.SuccessMessage.style.display =
                'none';


            Attandance.ForgotPassword.ErrorMessage.style.display =
                'block';


            // Keep administrator hidden

            Attandance.ForgotPassword.AdministratorSection.style.display =
                'none';

        }

    },

    // ---------------------------------------------------------
    // Send Mail - Open Gmail Compose
    // ---------------------------------------------------------

    SendEmail: function (e) {

        e.preventDefault();


        // Get entered CAPTCHA

        var entered =
            Attandance.ForgotPassword.CaptchaInput.value
                .trim()
                .toUpperCase();


        // Final CAPTCHA verification

        if (
            entered !==
            Attandance.ForgotPassword.CaptchaCode
        ) {

            Attandance.ForgotPassword.CaptchaInput.focus();

            Attandance.ForgotPassword.ErrorMessage.textContent =
                'Please complete the CAPTCHA verification first.';

            Attandance.ForgotPassword.ErrorMessage.style.display =
                'block';

            Attandance.ForgotPassword.AdministratorSection.style.display =
                'none';

            return;
        }


        // -----------------------------------------------------
        // Email Details
        // -----------------------------------------------------

        var email =
            'khurram.siddiqi@vasteksolutions.com';


        var subject =
            'VasTek Attendance System - Password Reset Request';


        var body =
            'Dear Administrator,\n\n' +

            'I have forgotten my password for the VasTek Attendance System and would like to request a password reset.\n\n' +

            'Employee ID / Username: \n' +

            'Employee Name: \n\n' +

            'Please reset my password and let me know once it has been completed.\n\n' +

            'Thank you.\n' +

            'VasTek Attendance System User';


        // -----------------------------------------------------
        // Gmail Compose URL
        // -----------------------------------------------------

        var gmailUrl =
            'https://mail.google.com/mail/?view=cm&fs=1' +
            '&to=' + encodeURIComponent(email) +
            '&su=' + encodeURIComponent(subject) +
            '&body=' + encodeURIComponent(body);


        // -----------------------------------------------------
        // Open Gmail Compose
        // -----------------------------------------------------

        window.location.href = gmailUrl;

    },


    // ---------------------------------------------------------
    // Initialize
    // ---------------------------------------------------------

    InitView: function () {

        Attandance.ForgotPassword.CaptchaElement =
            document.getElementById('captchaCode');


        Attandance.ForgotPassword.CaptchaInput =
            document.getElementById('captchaInput');


        Attandance.ForgotPassword.RefreshButton =
            document.getElementById('refreshCaptcha');


        Attandance.ForgotPassword.ShowCaptchaButton =
            document.getElementById('showCaptchaBtn');


        Attandance.ForgotPassword.CaptchaSection =
            document.getElementById('captchaSection');


        Attandance.ForgotPassword.AdministratorSection =
            document.getElementById('administratorSection');


        Attandance.ForgotPassword.SuccessMessage =
            document.getElementById('captchaSuccess');


        Attandance.ForgotPassword.ErrorMessage =
            document.getElementById('captchaError');


        Attandance.ForgotPassword.SendMailButton =
            document.getElementById('sendMailBtn');


        // -----------------------------------------------------
        // Page check
        // -----------------------------------------------------

        if (
            !Attandance.ForgotPassword.CaptchaElement ||
            !Attandance.ForgotPassword.CaptchaInput ||
            !Attandance.ForgotPassword.ShowCaptchaButton
        ) {

            return;

        }


        // -----------------------------------------------------
        // Contact Administrator
        // -----------------------------------------------------

        Attandance.ForgotPassword.ShowCaptchaButton
            .addEventListener(
                'click',
                function () {

                    Attandance.ForgotPassword.ShowCaptcha();

                }
            );


        // -----------------------------------------------------
        // CAPTCHA Input
        // -----------------------------------------------------

        Attandance.ForgotPassword.CaptchaInput
            .addEventListener(
                'input',
                function () {

                    Attandance.ForgotPassword.VerifyCaptcha();

                }
            );


        // -----------------------------------------------------
        // Refresh CAPTCHA
        // -----------------------------------------------------

        Attandance.ForgotPassword.RefreshButton
            .addEventListener(
                'click',
                function () {

                    Attandance.ForgotPassword.GenerateCaptcha();

                    Attandance.ForgotPassword.CaptchaInput.focus();

                }
            );


        // -----------------------------------------------------
        // Send Mail
        // -----------------------------------------------------

        if (Attandance.ForgotPassword.SendMailButton) {

            Attandance.ForgotPassword.SendMailButton
                .addEventListener(
                    'click',
                    function (e) {

                        Attandance.ForgotPassword.SendEmail(e);

                    }
                );

        }

    }

};
// =============================================================
// Monthly Report - Employee dropdown change par auto-submit
// =============================================================

Attandance.MonthlyReport = {

    EmpSelect: null,

    InitView: function () {

        Attandance.MonthlyReport.EmpSelect = document.getElementById('reportEmployeeId');

        if (!Attandance.MonthlyReport.EmpSelect) {
            return;
        }

        Attandance.MonthlyReport.EmpSelect.addEventListener('change', function () {
            Attandance.MonthlyReport.EmpSelect.closest('form').submit();
        });

    }

};
// =============================================================
// Document Ready
// =============================================================

document.addEventListener('DOMContentLoaded', function () {

    Attandance.Confirm.Init();

    Attandance.Login.InitView();

    Attandance.AssignLeave.InitView();

    Attandance.LeaveRequest.InitView();

    Attandance.Employee.InitView();

    Attandance.EmployeeFilter.InitView();

    Attandance.LeaveBalance.InitView();

    Attandance.ForgotPassword.InitView();

    Attandance.MonthlyReport.InitView();


});
//document.querySelectorAll('.employee-row').forEach(function (row) {
//    row.addEventListener('click', function () {

//        document.querySelectorAll('.employee-row')
//            .forEach(r => r.classList.remove('selected'));

//        row.classList.add('selected');

//        document.getElementById('empId').value =
//            row.dataset.id;

//        document.getElementById('empFullName').value =
//            row.dataset.fullname;

//        document.getElementById('empUsername').value =
//            row.dataset.username;

//        document.getElementById('empRole').value =
//            row.dataset.role;

//        document.getElementById('formTitle').innerText =
//            'Edit Employee';

//        document.getElementById('submitBtn').innerText =
//            'Update';

//        document.getElementById('cancelWrap').style.display =
//            'block';

//        document.getElementById('passwordHint').classList.remove('d-none');

//        document.getElementById('newEmpPassword').required = false;

//        document.getElementById('employeeForm').action =
//            document.getElementById('employeeForm').dataset.updateUrl;
//    });
//});
//document.getElementById('cancelEditBtn').addEventListener('click', function () {

//    document.querySelectorAll('.employee-row')
//        .forEach(r => r.classList.remove('selected'));

//    document.getElementById('empId').value = '0';
//    document.getElementById('empFullName').value = '';
//    document.getElementById('empUsername').value = '';
//    document.getElementById('newEmpPassword').value = '';
//    document.getElementById('empRole').value = 'Employee';

//    document.getElementById('formTitle').innerText =
//        'Add New Employee';

//    document.getElementById('submitBtn').innerText =
//        'Add';

//    document.getElementById('cancelWrap').style.display =
//        'none';

//    document.getElementById('passwordHint').classList.add('d-none');

//    document.getElementById('newEmpPassword').required = true;

//    document.getElementById('employeeForm').action =
//        document.getElementById('employeeForm').dataset.createUrl;
//});
// Password show/hide toggle - kahin bhi <button class="toggle-password" data-target="inputId"> use karen
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toggle-password').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = document.getElementById(btn.dataset.target);
            if (!input) return;
            var icon = btn.querySelector('i');
            if (input.type === 'password') {
                input.type = 'text';
                if (icon) { icon.classList.remove('bi-eye'); icon.classList.add('bi-eye-slash'); }
            } else {
                input.type = 'password';
                if (icon) { icon.classList.remove('bi-eye-slash'); icon.classList.add('bi-eye'); }
            }
        });
    });
});

// =============================================================
// Page Show
// =============================================================

window.addEventListener('pageshow', function () {

    Attandance.Login.HideLoader();

});


// =============================================================
// Page Load
// =============================================================

window.addEventListener('load', function () {

    Attandance.Login.HideLoader();

});