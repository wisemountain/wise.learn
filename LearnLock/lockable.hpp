#pragma once 

#include <shared_mutex>
#include <string_view>

namespace learn
{

class lockable : public std::shared_mutex
{
public:
  lockable(const char* name)
    : std::shared_mutex()
    , name_(name)
  {}

  const std::string_view& name() const
  {
    return name_;
  }

private: 
  std::string_view name_;
};

} // learn