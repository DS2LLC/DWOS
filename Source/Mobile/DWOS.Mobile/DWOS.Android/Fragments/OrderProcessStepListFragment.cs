using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DWOS.ViewModels;
using DWOS.Utilities;
using DWOS.Services.Messages;

namespace DWOS.Android
{
    /// <summary>
    /// Shows a list of process steps.
    /// </summary>
    public class OrderProcessStepListFragment : ListFragment
    {
        #region Fields

        public const string ORDER_PROCESS_STEP_FRAGMENT_TAG = "OrderProcessStepFragment";

        const string BUNDLEID_LISTVIEWSTATE = "stepsListViewState";
        const string BUNDLEID_MODE = "CurrentMode";
        const string BUNDLEID_CURRENTSTEP = "CurrentStep";
        IParcelable _listViewState;
        ProcessViewModel _processViewModel;
        BatchProcessViewModel _batchProcessViewModel;

        #endregion

        #region Properties

        public Mode CurrentMode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called to do initial creation of a fragment.
        /// </summary>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _processViewModel = ServiceContainer.Resolve<ProcessViewModel>();
            _batchProcessViewModel = ServiceContainer.Resolve<BatchProcessViewModel>();

            if (savedInstanceState != null)
            {
                _listViewState = savedInstanceState.GetParcelable(BUNDLEID_LISTVIEWSTATE) as IParcelable;
                CurrentMode = (Mode)savedInstanceState.GetInt(BUNDLEID_MODE);
            }
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            LoadStepList();
        }

        /// <summary>
        /// This method will be called when an item in the list is selected.
        /// </summary>
        /// <param name="l">The ListView where the click happened</param>
        /// <param name="view">The view that was clicked within the ListView</param>
        /// <param name="position">The position of the view in the list</param>
        /// <param name="id">The row id of the item that was clicked</param>
        public override void OnListItemClick(ListView listView, View view, int position, long id)
        {
            base.OnListItemClick(listView, view, position, id);
            var adapter = ListAdapter as ProcessStepsAdapter;
            if (adapter != null)
                adapter.NotifyDataSetChanged();

            var parentFragmentListener = Activity as IOrderProcessStepListCallback;
            if (parentFragmentListener != null)
                parentFragmentListener.OnStepSelected((int)id);

            _listViewState = ListView.OnSaveInstanceState();
        }

        /// <summary>
        /// Called when the fragment's activity has been created and this
        /// fragment's view hierarchy instantiated.
        /// </summary>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            ListView.ChoiceMode = ChoiceMode.Single;
        }

        /// <summary>
        /// Called when the fragment is visible to the user and actively running.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            
            RegisterViewModelEvents();
        }

        public override void OnPause()
        {
            base.OnPause();
            UnregisterViewModelEvents();
        }

        /// <summary>
        /// Called to ask the fragment to save its current dynamic state, so it
        /// can later be reconstructed in a new instance of its process is
        /// restarted.
        /// </summary>
        /// <param name="outState">Bundle in which to place your saved state.</param>
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutParcelable(BUNDLEID_LISTVIEWSTATE, ListView.OnSaveInstanceState());
            outState.PutInt(BUNDLEID_MODE, (int)CurrentMode);
        }

        private void LoadStepList()
        {
            var processSteps = CurrentMode == Mode.Orders ? _processViewModel.Process?.ProcessSteps : _batchProcessViewModel.Process?.ProcessSteps;
            if (processSteps != null)
            {
                ListAdapter = new ProcessStepsAdapter(Activity, processSteps, CurrentMode);

                if (_listViewState != null)
                    ListView.OnRestoreInstanceState(_listViewState);
                else if (ListAdapter.Count > 0)
                {
                    ProcessStepInfo selectedProcessStep = null;
                    if (CurrentMode == Mode.Orders)
                    {
                        var firstIncompleteStep = _processViewModel.GetFirstIncompleteStep(
                          _processViewModel.Process.ProcessSteps);
                        if (firstIncompleteStep != null)
                        {
                            _processViewModel.SetNextStep(firstIncompleteStep.ProcessStepId);
                            selectedProcessStep = _processViewModel.ProcessStep;
                        }
                    }
                    else
                    {
                        var firstIncompleteStep = _batchProcessViewModel.GetFirstIncompleteStep(
                              _batchProcessViewModel.Process.ProcessSteps);
                        if (firstIncompleteStep != null)
                        {
                            _batchProcessViewModel.SetNextStep(firstIncompleteStep.ProcessStepId);
                            selectedProcessStep = _batchProcessViewModel.ProcessStep;
                        }
                    }

                    if (selectedProcessStep != null)
                        SetListViewSelection(selectedProcessStep);
                }
            }
            else
                ListAdapter = null;
        }

        public void SetListViewSelection(ProcessStepInfo step)
        {
            if (step == null)
                throw new ArgumentNullException("step");
            int currentPosition = CurrentMode == Mode.Orders ? _processViewModel.Process.ProcessSteps.IndexOf(step)
                : _batchProcessViewModel.Process.ProcessSteps.IndexOf(step);
            if (currentPosition > -1)
            {
                ListView.SetSelection(currentPosition);
                ListView.SetItemChecked(currentPosition, value:true);
            }

            _listViewState = ListView.OnSaveInstanceState();
        }

        /// <summary>
        /// Called when it is time to register to view model events.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void RegisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
                _processViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            else
                _batchProcessViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void UnregisterViewModelEvents()
        {
            if (CurrentMode == Mode.Orders)
                _batchProcessViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            else
                _batchProcessViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Process")
                LoadStepList();
        }

        internal void RequestRefresh()
        {
            var adapter = ListAdapter as ProcessStepsAdapter;
            if (adapter != null)
                adapter.NotifyDataSetChanged();
        }

        #endregion
    }
}