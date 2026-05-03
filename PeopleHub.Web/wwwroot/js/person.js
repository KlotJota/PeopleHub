$(document).ready(function () {
    function PeopleViewModel() {
        var self = this;

        self.people = ko.observableArray([]);
        self.searchQuery = ko.observable('').extend({ rateLimit: 600 });

        self.isEditing = ko.observable(false);
        self.personToDelete = ko.observable(null);
        self.showErrors = ko.observable(false);

        self.currentPage = ko.observable(1);
        self.pageSize = 10;
        self.totalCount = ko.observable(0);

        self.selectedPerson = {
            id: ko.observable(),
            name: ko.observable(''),
            cpf: ko.observable(''),
            email: ko.observable(''),
            birthDate: ko.observable('')
        };

        self.isEmailValid = ko.computed(function () {
            var email = self.selectedPerson.email() || "";
            if (!email) return true;
            var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        });

        self.loadPeople = function () {
            var query = self.searchQuery() || "";
            var url = "/api/people?search=" + encodeURIComponent(query) +
                "&page=" + self.currentPage() +
                "&pageSize=" + self.pageSize;

            $.getJSON(url, function (data) {
                self.people(data.items);
                self.totalCount(data.totalCount);
            }).fail(function () {
                showToast("Erro ao carregar os dados.", "error");
            });
        };

        self.nextPage = function () {
            if ((self.currentPage() * self.pageSize) < self.totalCount()) {
                self.currentPage(self.currentPage() + 1);
                self.loadPeople();
            }
        };

        self.prevPage = function () {
            if (self.currentPage() > 1) {
                self.currentPage(self.currentPage() - 1);
                self.loadPeople();
            }
        };

        self.searchQuery.subscribe(function () {
            self.currentPage(1);
            self.loadPeople();
        });

        self.openAdd = function () {
            self.showErrors(false);
            self.isEditing(false);
            self.resetForm();
            document.getElementById('personModal').classList.remove('hidden');
        };

        self.openEdit = function (person) {
            self.showErrors(false);
            self.isEditing(true);
            self.selectedPerson.id(person.id || person.Id);
            self.selectedPerson.name(person.name || person.Name);
            self.selectedPerson.cpf(person.cpf || person.Cpf);
            self.selectedPerson.email(person.email || person.Email);

            var bDate = person.birthDate || person.BirthDate;
            if (bDate) {
                self.selectedPerson.birthDate(bDate.split('T')[0]);
            }

            document.getElementById('personModal').classList.remove('hidden');
        };

        self.closeModal = function () {
            document.getElementById('personModal').classList.add('hidden');
        };

        self.resetForm = function () {
            self.selectedPerson.id(null);
            self.selectedPerson.name('');
            self.selectedPerson.cpf('');
            self.selectedPerson.email('');
            self.selectedPerson.birthDate('');
        };

        function getCleanCpf(cpf) {
            return (cpf || "").replace(/\D/g, '');
        }

        self.validateForm = function (personData) {
            var cleanCpf = getCleanCpf(personData.cpf);

            if (!personData.name || cleanCpf.length !== 11 || !personData.email || !self.isEmailValid() || !personData.birthDate) {
                if (cleanCpf.length > 0 && cleanCpf.length !== 11) {
                    showToast("O CPF deve conter 11 dígitos.", "error");
                }
                return false;
            }

            var birth = new Date(personData.birthDate);
            var today = new Date();
            var age = today.getFullYear() - birth.getFullYear();
            var m = today.getMonth() - birth.getMonth();
            if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) age--;

            if (age < 18) {
                showToast("A pessoa deve ter no mínimo 18 anos.", "error");
                return false;
            }

            return true;
        };

        self.savePerson = function () {
            self.showErrors(true);

            var personData = {
                id: self.selectedPerson.id() || 0,
                name: self.selectedPerson.name(),
                cpf: getCleanCpf(self.selectedPerson.cpf()),
                email: self.selectedPerson.email(),
                birthDate: self.selectedPerson.birthDate()
            };

            if (!self.validateForm(personData)) return;

            var isUpdate = self.isEditing();

            $.ajax({
                url: isUpdate ? "/api/people/" + personData.id : "/api/people",
                type: isUpdate ? "PUT" : "POST",
                contentType: "application/json",
                data: JSON.stringify(personData),
                success: function () {
                    self.loadPeople();
                    self.closeModal();
                    showToast(isUpdate ? "Pessoa atualizada!" : "Pessoa adicionada com sucesso!");
                },
                error: function (xhr) {
                    var msg = xhr.responseText || "Erro ao salvar os dados da pessoa.";
                    showToast(msg, "error");
                }
            });
        };

        self.deletePerson = function (person) {
            self.personToDelete(person);
            document.getElementById('deleteModal').classList.remove('hidden');
        };

        self.confirmDelete = function () {
            var person = self.personToDelete();
            if (!person) return;
            var personId = person.id || person.Id;

            $.ajax({
                url: "/api/people/" + personId,
                type: "DELETE",
                success: function () {
                    self.loadPeople();
                    showToast("Registro removido com sucesso!");
                    self.personToDelete(null);
                    closeDeleteModal();
                },
                error: function () {
                    showToast("Erro ao remover o registro.", "error");
                    closeDeleteModal();
                }
            });
        };

        self.loadPeople();
    }

    window.showToast = function (message, type = "success") {
        const container = document.getElementById('toast-container');
        const toast = document.createElement('div');
        const isError = type === "error";
        const bgClass = isError ? "bg-red-50 border-red-200 text-red-800" : "bg-green-50 border-green-200 text-green-800";

        const iconSvg = isError
            ? `<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
           </svg>`
            : `<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M5 13l4 4L19 7" />
           </svg>`;

        toast.className = `flex items-center p-4 mb-4 rounded-lg border shadow-md transition-opacity duration-500 ${bgClass}`;
        toast.innerHTML = `${iconSvg}<span class="font-medium">${message}</span>`;
        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            setTimeout(() => toast.remove(), 500);
        }, 3000);
    };

    window.closeModal = function () {
        document.getElementById('personModal').classList.add('hidden');
    };

    window.closeDeleteModal = function () {
        document.getElementById('deleteModal').classList.add('hidden');
    };

    $('#cpfInput').mask('000.000.000-00');

    try {
        var viewModel = new PeopleViewModel();
        ko.applyBindings(viewModel);
    } catch (e) {
        console.error(e);
        showToast("Erro crítico ao inicializar o sistema.", "error");
    }
});
