#pragma once

#include "MainPage.g.h"

#include "winrt/SampleComponent.h"

#include <windows.h>
#include <evntprov.h>

namespace winrt::TestApp::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        winrt::SampleComponent::Example myExample;

        MainPage()
        {
            // Xaml objects should not call InitializeComponent during construction.
            // See https://github.com/microsoft/cppwinrt/tree/master/nuget#initializecomponent
            myExample.Init();
            OutputDebugString(TEXT("initititititi 0000\n"));
        }

        void ClickHandler(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& args);
    };
}

namespace winrt::TestApp::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
