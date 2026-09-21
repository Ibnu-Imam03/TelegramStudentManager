// ============================================================
// TELEGRAM MINI APP
// ============================================================

const tg = window.Telegram?.WebApp;

if (tg) {

    tg.ready();

    tg.expand();

}


// ============================================================
// API URL
// ============================================================

// Your existing ASP.NET Core API
const API_URL = "https://telegram-student-manager-1.onrender.com/api/Students";

// ============================================================
// GLOBAL VARIABLES
// ============================================================

let students = [];

let editingStudentID = null;


// ============================================================
// PAGE ELEMENTS
// ============================================================

const dashboardPage =
    document.getElementById("dashboardPage");

const studentsPage =
    document.getElementById("studentsPage");

const detailsPage =
    document.getElementById("detailsPage");

const formPage =
    document.getElementById("formPage");


// ============================================================
// START APPLICATION
// ============================================================

document.addEventListener(
    "DOMContentLoaded",
    function () {

        loadStudents();

        loadTelegramUser();

    }
);


// ============================================================
// TELEGRAM USER
// ============================================================

function loadTelegramUser() {

    if (!tg)
        return;

    const user =
        tg.initDataUnsafe?.user;

    if (!user)
        return;

    const name =
        user.first_name || "User";

    document.getElementById(
        "welcomeText"
    ).textContent =
        `Welcome, ${name} 👋`;
}


// ============================================================
// LOAD ALL STUDENTS
// ============================================================

async function loadStudents() {

    try {

        const response =
            await fetch(API_URL);


        if (!response.ok) {

            throw new Error(
                "Failed to load students."
            );

        }


        students =
            await response.json();


        // Update count

        document.getElementById(
            "studentCount"
        ).textContent =
            students.length;


        // Display dashboard students

        displayRecentStudents();


        // Display student page

        displayStudents(students);


    }
    catch (error) {

        console.error(error);

        showMessage(
            "❌ Could not connect to the API.",
            "error"
        );

        document.getElementById(
            "studentList"
        ).innerHTML = `
            <div class="empty">
                <div class="empty-icon">⚠️</div>
                <p>Could not load students.</p>
            </div>
        `;

    }

}


// ============================================================
// DISPLAY RECENT STUDENTS
// ============================================================

function displayRecentStudents() {

    const container =
        document.getElementById(
            "recentStudents"
        );


    if (students.length === 0) {

        container.innerHTML = `
            <div class="empty">
                <div class="empty-icon">👨‍🎓</div>
                <p>No students found.</p>
            </div>
        `;

        return;

    }


    // Show maximum 5

    const recent =
        students.slice(0, 5);


    container.innerHTML =
        recent.map(student => `

            <div
                class="student-card"
                onclick="viewStudent(${student.studentID})">

                <div class="student-main">

                    <div class="student-avatar">
                        👤
                    </div>

                    <div class="student-info">

                        <h3>
                            ${escapeHtml(student.fullName)}
                        </h3>

                        <p>
                            ${escapeHtml(student.department)}
                            • Year ${student.year}
                        </p>

                    </div>

                    <div class="student-arrow">
                        →
                    </div>

                </div>

            </div>

        `).join("");

}


// ============================================================
// DISPLAY STUDENTS
// ============================================================

function displayStudents(list) {

    const container =
        document.getElementById(
            "studentList"
        );


    if (list.length === 0) {

        container.innerHTML = `
            <div class="empty">

                <div class="empty-icon">
                    🔍
                </div>

                <p>
                    No students found.
                </p>

            </div>
        `;

        return;

    }


    container.innerHTML =
        list.map(student => `

            <div class="student-card">

                <div
                    class="student-main"
                    onclick="viewStudent(${student.studentID})">

                    <div class="student-avatar">
                        👤
                    </div>


                    <div class="student-info">

                        <h3>
                            ${escapeHtml(student.fullName)}
                        </h3>

                        <p>
                            ${escapeHtml(student.studentNumber)}
                            •
                            ${escapeHtml(student.department)}
                        </p>

                    </div>


                    <div class="student-arrow">
                        →
                    </div>

                </div>


                <div class="student-actions">

                    <button
                        class="edit-button"
                        onclick="event.stopPropagation(); editStudent(${student.studentID})">

                        ✏️ Edit

                    </button>


                    <button
                        class="delete-button"
                        onclick="event.stopPropagation(); deleteStudent(${student.studentID})">

                        🗑️ Delete

                    </button>

                </div>

            </div>

        `).join("");

}


// ============================================================
// SEARCH STUDENTS
// ============================================================

function searchStudents() {

    const search =
        document.getElementById(
            "searchInput"
        ).value
            .toLowerCase()
            .trim();


    if (search === "") {

        displayStudents(students);

        return;

    }


    const filtered =
        students.filter(student =>

            student.fullName
                .toLowerCase()
                .includes(search)

            ||

            student.studentNumber
                .toLowerCase()
                .includes(search)

            ||

            student.department
                .toLowerCase()
                .includes(search)

            ||

            student.phone
                .toLowerCase()
                .includes(search)

        );


    displayStudents(filtered);

}


// ============================================================
// GET ONE STUDENT
// ============================================================

async function getStudent(studentID) {

    const response =
        await fetch(
            `${API_URL}/${studentID}`
        );


    if (!response.ok) {

        throw new Error(
            "Student not found."
        );

    }


    return await response.json();

}


// ============================================================
// VIEW STUDENT
// ============================================================

async function viewStudent(studentID) {

    try {

        const student =
            await getStudent(studentID);


        document.getElementById(
            "studentDetails"
        ).innerHTML = `

            <div class="details-card">

                <div class="details-top">

                    <div class="details-avatar">
                        👤
                    </div>

                    <h2>
                        ${escapeHtml(student.fullName)}
                    </h2>

                    <p>
                        ${escapeHtml(student.department)}
                    </p>

                </div>


                <div class="detail-row">

                    <span class="detail-label">
                        Student ID
                    </span>

                    <span class="detail-value">
                        ${student.studentID}
                    </span>

                </div>


                <div class="detail-row">

                    <span class="detail-label">
                        Student Number
                    </span>

                    <span class="detail-value">
                        ${escapeHtml(student.studentNumber)}
                    </span>

                </div>


                <div class="detail-row">

                    <span class="detail-label">
                        Department
                    </span>

                    <span class="detail-value">
                        ${escapeHtml(student.department)}
                    </span>

                </div>


                <div class="detail-row">

                    <span class="detail-label">
                        Year
                    </span>

                    <span class="detail-value">
                        Year ${student.year}
                    </span>

                </div>


                <div class="detail-row">

                    <span class="detail-label">
                        Phone
                    </span>

                    <span class="detail-value">
                        ${escapeHtml(student.phone)}
                    </span>

                </div>


                <div class="student-actions">

                    <button
                        class="edit-button"
                        onclick="editStudent(${student.studentID})">

                        ✏️ Edit

                    </button>


                    <button
                        class="delete-button"
                        onclick="deleteStudent(${student.studentID})">

                        🗑️ Delete

                    </button>

                </div>

            </div>

        `;


        showPage("details");

    }
    catch (error) {

        console.error(error);

        showMessage(
            "❌ Could not load student.",
            "error"
        );

    }

}


// ============================================================
// OPEN ADD FORM
// ============================================================

function openAddStudent() {

    editingStudentID = null;


    document.getElementById(
        "formTitle"
    ).textContent =
        "Add Student";


    document.getElementById(
        "formSubtitle"
    ).textContent =
        "Enter student information";


    document.getElementById(
        "saveButton"
    ).textContent =
        "Add Student";


    document.getElementById(
        "studentForm"
    ).reset();


    document.getElementById(
        "studentID"
    ).value = "";


    showPage("form");

}


// ============================================================
// EDIT STUDENT
// ============================================================

async function editStudent(studentID) {

    try {

        const student =
            await getStudent(studentID);


        editingStudentID =
            student.studentID;


        document.getElementById(
            "studentID"
        ).value =
            student.studentID;


        document.getElementById(
            "fullName"
        ).value =
            student.fullName;


        document.getElementById(
            "studentNumber"
        ).value =
            student.studentNumber;


        document.getElementById(
            "department"
        ).value =
            student.department;


        document.getElementById(
            "year"
        ).value =
            student.year;


        document.getElementById(
            "phone"
        ).value =
            student.phone;


        document.getElementById(
            "formTitle"
        ).textContent =
            "Update Student";


        document.getElementById(
            "formSubtitle"
        ).textContent =
            "Update student information";


        document.getElementById(
            "saveButton"
        ).textContent =
            "Update Student";


        showPage("form");

    }
    catch (error) {

        console.error(error);

        showMessage(
            "❌ Could not load student.",
            "error"
        );

    }

}


// ============================================================
// ADD OR UPDATE STUDENT
// ============================================================

async function saveStudent(event) {

    event.preventDefault();


    const student = {

        fullName:
            document.getElementById(
                "fullName"
            ).value.trim(),

        studentNumber:
            document.getElementById(
                "studentNumber"
            ).value.trim(),

        department:
            document.getElementById(
                "department"
            ).value.trim(),

        year:
            parseInt(
                document.getElementById(
                    "year"
                ).value
            ),

        phone:
            document.getElementById(
                "phone"
            ).value.trim()

    };


    // ========================================
    // VALIDATION
    // ========================================

    if (!student.fullName ||
        !student.studentNumber ||
        !student.department ||
        !student.year ||
        !student.phone) {

        showMessage(
            "❌ Please fill in all fields.",
            "error"
        );

        return;

    }


    try {

        let response;


        // ====================================
        // ADD
        // ====================================

        if (editingStudentID === null) {

            response =
                await fetch(
                    API_URL,
                    {
                        method: "POST",

                        headers: {
                            "Content-Type":
                                "application/json"
                        },

                        body:
                            JSON.stringify(student)
                    }
                );

        }


        // ====================================
        // UPDATE
        // ====================================

        else {

            response =
                await fetch(
                    `${API_URL}/${editingStudentID}`,

                    {
                        method: "PUT",

                        headers: {
                            "Content-Type":
                                "application/json"
                        },

                        body:
                            JSON.stringify(student)
                    }
                );

        }


        // ====================================
        // CHECK RESPONSE
        // ====================================

        if (!response.ok) {

            const errorText =
                await response.text();

            console.error(errorText);

            throw new Error(
                "Save failed."
            );

        }


        // ====================================
        // SUCCESS
        // ====================================

        if (editingStudentID === null) {

            showMessage(
                "✅ Student added successfully.",
                "success"
            );

        }
        else {

            showMessage(
                "✅ Student updated successfully.",
                "success"
            );

        }


        // Reload data

        await loadStudents();


        // Return to students

        showStudentsPage();


    }
    catch (error) {

        console.error(error);

        showMessage(
            "❌ Could not save student.",
            "error"
        );

    }

}


// ============================================================
// DELETE STUDENT
// ============================================================

async function deleteStudent(studentID) {

    const student =
        students.find(
            s => s.studentID === studentID
        );


    if (!student)
        return;


    const confirmed =
        confirm(
            `Are you sure you want to delete "${student.fullName}"?`
        );


    if (!confirmed)
        return;


    try {

        const response =
            await fetch(
                `${API_URL}/${studentID}`,

                {
                    method: "DELETE"
                }
            );


        if (!response.ok) {

            throw new Error(
                "Delete failed."
            );

        }


        showMessage(
            "✅ Student deleted successfully.",
            "success"
        );


        await loadStudents();


        showStudentsPage();

    }
    catch (error) {

        console.error(error);

        showMessage(
            "❌ Could not delete student.",
            "error"
        );

    }

}


// ============================================================
// SHOW HOME PAGE
// ============================================================

function showHomePage() {

    showPage("dashboard");

}


// ============================================================
// SHOW STUDENTS PAGE
// ============================================================

function showStudentsPage() {

    showPage("students");

}


// ============================================================
// SHOW PAGE
// ============================================================

function showPage(pageName) {

    // Hide all

    dashboardPage.classList.remove(
        "active"
    );

    studentsPage.classList.remove(
        "active"
    );

    detailsPage.classList.remove(
        "active"
    );

    formPage.classList.remove(
        "active"
    );


    // Show selected

    if (pageName === "dashboard") {

        dashboardPage.classList.add(
            "active"
        );

    }


    else if (pageName === "students") {

        studentsPage.classList.add(
            "active"
        );

    }


    else if (pageName === "details") {

        detailsPage.classList.add(
            "active"
        );

    }


    else if (pageName === "form") {

        formPage.classList.add(
            "active"
        );

    }


    // Navigation

    updateNavigation(pageName);

}


// ============================================================
// UPDATE NAVIGATION
// ============================================================

function updateNavigation(pageName) {

    document.getElementById(
        "homeNav"
    ).classList.remove("active");


    document.getElementById(
        "studentsNav"
    ).classList.remove("active");


    if (pageName === "dashboard") {

        document.getElementById(
            "homeNav"
        ).classList.add("active");

    }


    if (pageName === "students") {

        document.getElementById(
            "studentsNav"
        ).classList.add("active");

    }

}


// ============================================================
// MESSAGE
// ============================================================

function showMessage(
    text,
    type = ""
) {

    const message =
        document.getElementById(
            "message"
        );


    message.textContent = text;


    message.className =
        `message show ${type}`;


    setTimeout(
        function () {

            message.classList.remove(
                "show"
            );

        },

        3000
    );

}


// ============================================================
// SECURITY
// ============================================================

function escapeHtml(value) {

    if (value === null ||
        value === undefined) {

        return "";

    }


    return String(value)

        .replace(
            /&/g,
            "&amp;"
        )

        .replace(
            /</g,
            "&lt;"
        )

        .replace(
            />/g,
            "&gt;"
        )

        .replace(
            /"/g,
            "&quot;"
        )

        .replace(
            /'/g,
            "&#039;"
        );

}