using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VeterinaryModel;

namespace ProjectMedii_Istrate_Bogdan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        VeterinaryEntitiesModel ctx = new VeterinaryEntitiesModel();
        CollectionViewSource animalViewSource;
        CollectionViewSource customerViewSource;
        CollectionViewSource customerAppointmentsViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            animalViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("animalViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // animalViewSource.Source = [generic data source]
            animalViewSource.Source = ctx.Animals.Local;
            ctx.Animals.Load();

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
            customerViewSource.Source = ctx.Customers.Local;
            
            customerAppointmentsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerAppointmentsViewSource")));
            //customerAppointmentsViewSource.Source = ctx.Appointments.Local;

            ctx.Customers.Load();
            ctx.Appointments.Load();
            ctx.Animals.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbAnimal.ItemsSource = ctx.Animals.Local;
            //cmbAnimal.DisplayMemberPath = "Name";
            cmbAnimal.SelectedValuePath = "PetId";
            BindDataGrid();
        }
        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Animal animal = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Animal entity
                    animal = new Animal()
                    {
                        Name = nameTextBox.Text.Trim(),
                        Breed = breedTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Animals.Add(animal);
                    animalViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnDelete1.IsEnabled = true;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                nameTextBox.IsEnabled = false;
                breedTextBox.IsEnabled = false;
                SetValidationBinding();

            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    animal = (Animal)animalDataGrid.SelectedItem;
                    animal.Name = nameTextBox.Text.Trim();
                    animal.Breed = breedTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                animalViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                animalViewSource.View.MoveCurrentTo(animal);
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;

                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;

                nameTextBox.IsEnabled = false;
                breedTextBox.IsEnabled = false;
                SetValidationBinding();

            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    animal = (Animal)animalDataGrid.SelectedItem;
                    ctx.Animals.Remove(animal);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                animalViewSource.View.Refresh();
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;

                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                nameTextBox.IsEnabled = false;
                breedTextBox.IsEnabled = false;
            }
            SetValidationBinding();
        }
        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            animalViewSource.View.MoveCurrentToPrevious();

        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            animalViewSource.View.MoveCurrentToNext();
        }
        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;

            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            breedTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(breedTextBox, TextBox.TextProperty);

            nameTextBox.Text = "";
            breedTextBox.Text = "";

            Keyboard.Focus(nameTextBox);
            SetValidationBinding();

        }
        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;

            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            nameTextBox.IsEnabled = true;
            breedTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(breedTextBox, TextBox.TextProperty);

            Keyboard.Focus(nameTextBox);
            SetValidationBinding();
        }
        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;

            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            BindingOperations.ClearBinding(nameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(breedTextBox, TextBox.TextProperty);
        }
        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;

            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            nameTextBox.IsEnabled = false;
            breedTextBox.IsEnabled = false;

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnDelete.IsEnabled = true;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                SetValidationBinding();

            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                SetValidationBinding();

            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;

                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            SetValidationBinding();
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";

            Keyboard.Focus(firstNameTextBox);
            SetValidationBinding();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            Keyboard.Focus(firstNameTextBox);
            SetValidationBinding();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;

            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
        }
        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Appointment appointment = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Animal animal = (Animal)cmbAnimal.SelectedItem;
                    //instantiem Appointment entity
                    appointment = new Appointment()
                    {

                        CustId = customer.CustId,
                        PetId = animal.PetId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Appointments.Add(appointment);
                    ctx.SaveChanges();
                    BindDataGrid();
                    customerAppointmentsViewSource.View.Refresh();
                    //salvam modificarile
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnDelete2.IsEnabled = true;
                btnCancel2.IsEnabled = false;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbAnimal.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedAppointment = appointmentsDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedAppointment.AppointmentId;
                    var editedAppointment = ctx.Appointments.FirstOrDefault(s => s.AppointmentId == curr_id);
                    if (editedAppointment != null)
                    {
                        editedAppointment.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedAppointment.PetId = Convert.ToInt32(cmbAnimal.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerAppointmentsViewSource.View.Refresh();
                customerViewSource.View.MoveCurrentTo(selectedAppointment);

                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;

                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbAnimal.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedAppointment = appointmentsDataGrid.SelectedItem;
                    int curr_id = selectedAppointment.AppointmentId;
                    var deletedAppointment = ctx.Appointments.FirstOrDefault(s => s.AppointmentId == curr_id);
                    if (deletedAppointment != null)
                    {
                        ctx.Appointments.Remove(deletedAppointment);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerAppointmentsViewSource.View.Refresh();
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;

                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbAnimal.IsEnabled = false;
            }
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;

            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbAnimal.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbAnimal, TextBox.TextProperty);

            Keyboard.Focus(cmbCustomers);
        }
        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;

            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbCustomers.IsEnabled = true;
            cmbAnimal.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbAnimal, TextBox.TextProperty);

            Keyboard.Focus(cmbCustomers);
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;

            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            BindingOperations.ClearBinding(cmbCustomers, TextBox.TextProperty);
            BindingOperations.ClearBinding(cmbAnimal, TextBox.TextProperty);
        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnDelete2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;

            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;
            cmbCustomers.IsEnabled = false;
            cmbAnimal.IsEnabled = false;
        }
        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            customerAppointmentsViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            customerAppointmentsViewSource.View.MoveCurrentToNext();
        }
        private void BindDataGrid()
        {
            var queryAppointment = from app in ctx.Appointments
                             join cust in ctx.Customers on app.CustId equals
                             cust.CustId
                             join ani in ctx.Animals on app.PetId
                 equals ani.PetId
                             select new
                             {
                                 app.AppointmentId,
                                 app.PetId,
                                 app.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 ani.Name,
                                 ani.Breed
                             };
            customerAppointmentsViewSource.Source = queryAppointment.ToList();
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty,
           firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty,
           lastNameValidationBinding); //setare binding nou
        }

    }

}


